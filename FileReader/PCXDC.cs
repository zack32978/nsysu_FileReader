using System;
using System.Drawing;
using System.Drawing.Imaging;
namespace PCXDecoder
{
    class PReadHeader
    {
        public byte[] header = new byte[128];
        public byte[] colormap = new byte[48];
        public byte[] filler = new byte[58];
        public void readheader(byte[] imagefile)
        {
            Array.Copy(imagefile, header, 128);
        }
        public byte Manfacturer { get { return header[0]; } }                                               //10 = Zshoft.pcx 
        public byte Version { get { return header[1]; } }                                                   //5 = color palette
        public byte Encoding { get { return header[2]; } }
        public byte BitPerPixel { get { return header[3]; } }
        public int Xmin { get { return header[5]*256 + header[4]; } }
        public int Ymin { get { return header[7] * 256 + header[6]; } }
        public int Xmax { get { return header[9] * 256 + header[8]; } }
        public int Ymax { get { return header[11] * 256 + header[10]; } }
        public int Hdpi { get { return header[13] * 256 + header[12]; } }
        public int Vdpi { get { return header[15] * 256 + header[14]; } }
        public byte[] Colormap { get { Array.Copy(header, 16, colormap, 0, 48); return colormap; } }        //1個顏色3bytes total 16種顏色
        public byte Reserved { get { return header[64]; } }
        public byte NPlanes { get { return header[65]; } }                                                  //1,3,4
        public int BytesPerLine { get { return header[67]*256 + header[66]; } }                             //BytesPerLine must be even
        public int PaletteInfo { get { return header[69]*256 + header[68]; } }                              //1 = Color.BW, 2 = Grayscale 
        public int HscreenSize { get { return header[71]*256 + header[70]; } }
        public int VscreenSize { get { return header[73]*256 + header[72]; } }
        public byte[] Filler { get { Array.Copy(header, 74, filler, 0, 58); return filler; } }            //set all byte to 0
        public int Width { get { return Xmax - Xmin + 1; } }
        public int Height { get { return Ymax - Ymin + 1; } }
    }
    class PDecodeFile
    {
        PReadHeader PCX_header = new PReadHeader();
        public Bitmap decode_image;
        int readindex;
        public void decoPixel(byte[] imagefile)
        {
            PCX_header.readheader(imagefile);
            if (PCX_header.NPlanes == 3)     //no palette?
            {
                int byteperline = PCX_header.BytesPerLine;
                decode_image = new Bitmap(byteperline, PCX_header.Height);
                decode_image = unpack3plane(imagefile);
            }
            else if (PCX_header.NPlanes == 1) //use palette
            {
                decode_image = new Bitmap(PCX_header.Width, PCX_header.Height);
                decode_image = unpackRLE(imagefile);
            }
        }
        //width = bytesperline , return a row of pixelvalue [RGB RGB RGB ....]
        public Bitmap unpack3plane(byte[] imagevalue)
        {
            PCX_header.readheader(imagevalue);
            int byteperline = PCX_header.BytesPerLine;
            byte[] pixelvalue = new byte[byteperline * 3 * PCX_header.Height];
            int pindex;
            int writeindex;
            Color RGB;
            Color[] pixelcolor = new Color[byteperline * PCX_header.Height];
            Bitmap imagepixel = new Bitmap(byteperline, PCX_header.Height);
            readindex = 128;
            for (int i = 0; i < PCX_header.Height; i++)                 //對回傳的array做height次 => [RGB RGB ...](bytesperline) * height
            {
                int rgb_index = 0;
                writeindex = byteperline * i;
                while (readindex < imagevalue.Length)
                {
                    byte bytevalue = imagevalue[readindex];
                    if ((bytevalue & 0xC0) == 0xC0)
                    {
                        int count = bytevalue & 0x3F;
                        readindex++;
                        for (int j = 0; j < count; j++)
                        {
                            if (j + writeindex >= byteperline * (i + 1))
                            {
                                j = 0;
                                writeindex = 0 + byteperline * i;
                                rgb_index++;
                                if (rgb_index == 3) break;
                            }
                            pindex = (j + writeindex) * 3 + rgb_index;
                            pixelvalue[pindex] = imagevalue[readindex];
                        }
                        writeindex += count;
                    }
                    else
                    {
                        pindex = writeindex * 3 + rgb_index;
                        pixelvalue[pindex] = bytevalue;
                        writeindex++;
                    }
                    if (writeindex >= byteperline * (i + 1))
                    {
                        writeindex = 0 + byteperline * i;
                        rgb_index++;
                    }
                    readindex++;
                    if (rgb_index == 3)
                        break;
                }
            }
            int RGBindex = 0;
            for (int j = 0; j < byteperline * PCX_header.Height; j++)
            {                                                       //pixelvalue的RGB值存成color[]，set給pixelcolor
                RGB = Color.FromArgb(pixelvalue[j * 3], pixelvalue[j * 3 + 1], pixelvalue[j * 3 + 2]);
                pixelcolor.SetValue(RGB, RGBindex);
                RGBindex++;
            }

            int index = 0;
            for (int m = 0; m < imagepixel.Height; m++)                 //setpixel給要輸出的array
            {
                for (int n = 0; n < imagepixel.Width; n++)
                {
                    imagepixel.SetPixel(n, m, pixelcolor[index]);
                    index++;
                }
            }
            return imagepixel;
        }
        //做RLE解碼然後去比對palette得到有pixelvalue的array
        public Bitmap unpackRLE(byte[] imagevalue)
        {
            PCX_header.readheader(imagevalue);
            PReadPalette read_palette = new PReadPalette();

            int pindex = 0;
            int readindex = 128;
            int writeindex = 0;
            int count;
            int size;
            size = PCX_header.Height * PCX_header.Width;
            byte[] pixelvalue = new byte[size];
            Bitmap imagepixel = new Bitmap(PCX_header.Width, PCX_header.Height);
            while (writeindex < size)
            {
                byte bytevalue = imagevalue[readindex];
                if ((bytevalue & 0xC0) == 0xC0)
                {
                    count = bytevalue & 0x3F;
                    readindex++;
                    for (int i = 0; i < count; i++)
                    {
                        pixelvalue[writeindex] = imagevalue[readindex];
                        writeindex++;
                    }
                }
                else
                {
                    pixelvalue[writeindex] = bytevalue;
                    writeindex++;
                }
                readindex++;
            }
            read_palette.readpalette(imagevalue);
            Color[] palette = read_palette.getPalette();
            for (int m = 0; m < imagepixel.Height; m++)
            {
                for (int n = 0; n < imagepixel.Width; n++)
                {
                    imagepixel.SetPixel(n, m, palette[pixelvalue[pindex]]);
                    pindex++;
                }
            }
            return imagepixel;
        }
        
    }
    //=======================Palette========================================== 
    class PReadPalette
    {
        byte[] imagePalette = new byte[768];
        PReadHeader PCX_header = new PReadHeader();
        public void readpalette(byte[] imagefile)
        {
            PCX_header.readheader(imagefile);
            Array.Copy(imagefile, imagefile.Length - 768, imagePalette, 0, 768);
        }
        public byte[] palettevalue { get { return imagePalette; } }
        public Color[] getPalette()
        {
            Color[] palette = new Color[256];
            for (int i = 0; i < 256; i++)
            {
                Color RGB = Color.FromArgb(imagePalette[i * 3], imagePalette[i * 3 + 1], imagePalette[i * 3 + 2]);
                palette.SetValue(RGB, i);
            }
            return palette;
        }
        public Color[] colormap()
        {
            Color[] colormap = new Color[16];
            Color RGB;
            for (int i = 0; i < 16; i++)
            {
                RGB = Color.FromArgb(PCX_header.Colormap[i * 3], PCX_header.Colormap[i * 3 + 1], PCX_header.Colormap[i * 3 + 2]);
                colormap.SetValue(RGB, i);
            }
            return colormap;
        }

    }
}





/*public void RGB2HSI(byte[] Red, byte[] Green, byte[] Blue, Bitmap imagepixel)
        {
            double[] H = new double[imagepixel.Height * imagepixel.Width];
            double[] S = new double[imagepixel.Height * imagepixel.Width];
            double[] I = new double[imagepixel.Height * imagepixel.Width];
            Color[] Hpixelcolor = new Color[imagepixel.Height * imagepixel.Width];
            Color[] Spixelcolor = new Color[imagepixel.Height * imagepixel.Width];
            Color[] Ipixelcolor = new Color[imagepixel.Height * imagepixel.Width];
            Color Hue;
            Color Saturation;
            Color Intensity;
            Color HSI;
            double value1, value2, radial;
            int theta;
            for (int i = 0; i < Red.Length; i++)
            {
                double R = (double)Red[i] / (Red[i] + Green[i] + Blue[i]);
                double G = (double)Green[i] / (Red[i] + Green[i] + Blue[i]);
                double B = (double)Blue[i] / (Red[i] + Green[i] + Blue[i]);
                value1 = 0.5 * ((R - G) + (R - B));
                value2 = Math.Pow((Math.Pow(R - G, 2) + (R - B) * (G - B)), 0.5);
                radial = Math.Acos(value1 / value2);
                theta = (int)(radial * 180.0 / Math.PI);

                if (!double.IsNaN(theta))
                    if (B <= G)
                        H[i] = theta;
                    else
                        H[i] = (360 - theta);
                else
                    H[i] = 0;
                S[i] = 1 - (3 * Math.Min(Math.Min(R, G), B) / (R + G + B));
                if (double.IsNaN(S[i]))
                    S[i] = 0;
                I[i] = Red[i] + Green[i] + Blue[i] / 3.0;

                Intensity = Color.FromArgb(Convert.ToInt32(I[i]), 0, 0);
                Saturation = Color.FromArgb(0, Convert.ToInt32(S[i]), 0);
                Hue = Color.FromArgb(0, 0, Convert.ToInt32(H[i]));
                Ipixelcolor.SetValue(Intensity, i);
                Spixelcolor.SetValue(Saturation, i);
                Hpixelcolor.SetValue(Hue, i);
            }
            int index = 0;
            for (int m = 0; m < imagepixel.Height; m++)
            {
                for (int n = 0; n < imagepixel.Width; n++)
                {

                    H_image.SetPixel(n, m, Hpixelcolor[index]);
                    S_image.SetPixel(n, m, Spixelcolor[index]);
                    I_image.SetPixel(n, m, Ipixelcolor[index]);
                }
            }
        }*/