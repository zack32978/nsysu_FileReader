using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace BMPDecoder
{
	public class BReadHeader
	{
        public byte[] header = new byte[14];
        public byte[] file_type = new byte[2];
        public byte[] Infoheader = new byte[40];
        public void readheader(byte[] imagefile)
        {
            Array.Copy(imagefile, header, 14);
            Array.Copy(imagefile, 14, Infoheader, 0, 40);
        }
        public byte[] FileType { get { Array.Copy(header, 0, file_type, 0, 2); return file_type; } }
        public int FileSize { get { return header[2] + header[3]*256 + header[4] * 256 * 256 + header[5] * 256 * 256 * 256; } }
        public int Reserved_0 { get { return header[6] + header[7] * 256; } }
        public int Reserved_1 { get { return header[8] + header[9] * 256; } }
        public int Offset { get { return header[10] + header[11] * 256 + header[12] * 256 * 256 + header[13] * 256 * 256 * 256; } }
        public int InfoHeaderSize { get { return Infoheader[0] + Infoheader[1] * 256 + Infoheader[2] * 256 * 256 + Infoheader[3] * 256 * 256 * 256; } }
        public int BitWidth { get { return Infoheader[4] + Infoheader[5] * 256 + Infoheader[6] * 256 * 256 + Infoheader[7] * 256 * 256 * 256; } }
        public int BitHeight { get { return Infoheader[8] + Infoheader[9] * 256 + Infoheader[10] * 256 * 256 + Infoheader[11] * 256 * 256 * 256; } }
        public int NPlanes { get { return  Infoheader[12]  + Infoheader[13] * 256; } }   // ==1
        public int BitsPerPixel { get { return Infoheader[14] + Infoheader[15] * 256; } }/*1：單色點陣圖（使用 2 色調色盤）
                                                                                           4：4 位元點陣圖（使用 16 色調色盤）
                                                                                           8：8 位元點陣圖（使用 256 色調色盤）
                                                                                           16：16 位元高彩點陣圖（不一定使用調色盤）
                                                                                           24：24 位元全彩點陣圖（不使用調色盤）
                                                                                           32：32 位元全彩點陣圖（不一定使用調色盤*/
        public int Compression { get { return Infoheader[16] + Infoheader[17] * 256 + Infoheader[18] * 256 * 256 + Infoheader[19] * 256 * 256 * 256; } }
        public int ImageSize { get { return Infoheader[20] + Infoheader[21] * 256 + Infoheader[22] * 256 * 256 + Infoheader[23] * 256 * 256 * 256; } }
        public int Hdpi { get { return Infoheader[24] + Infoheader[25] * 256 + Infoheader[26] * 256 * 256 + Infoheader[27] * 256 * 256 * 256; } }
        public int Vdpi { get { return Infoheader[28] + Infoheader[29] * 256 + Infoheader[30] * 256 * 256 + Infoheader[31] * 256 * 256 * 256; } }
        public int NColors { get { return Infoheader[32] + Infoheader[33] * 256 + Infoheader[34] * 256 * 256 + Infoheader[35] * 256 * 256 * 256; } }
        public int ImportantColors { get { return Infoheader[36] + Infoheader[37] * 256 + Infoheader[38] * 256 * 256 + Infoheader[39] * 256 * 256 * 256; } }

    }
    //Compression : 0(沒有壓縮，不使用調色盤), 1(RLE 8), 2(RLE 4), 3(bits per pixel =16 or32,BI_BITFIELDS)
    class BReadPalette
    {
        BReadHeader BMP_header = new BReadHeader();
        public byte[] imagePalette;
        public int palette_size;
        public void readpalette(byte[] imagefile)
        {
            BMP_header.readheader(imagefile);

            //將調色盤依檔案的header、info header計算位址存起來
            if (BMP_header.BitsPerPixel < 16 && BMP_header.Compression != 0)
            {
                palette_size = (int)Math.Pow(Convert.ToDouble(2), Convert.ToDouble(BMP_header.BitsPerPixel));
                //透過使用的顏色決定調色盤大小
                if (BMP_header.NColors == 0 || BMP_header.NColors == palette_size)
                {
                    imagePalette = new byte[palette_size * 4];
                }
                else
                {
                    imagePalette = new byte[BMP_header.NColors * 4];
                }
                Array.Copy(imagefile, 14 + BMP_header.InfoHeaderSize, imagePalette, 0, imagePalette.Length);
            }
            else if (BMP_header.BitsPerPixel == 32 || BMP_header.BitsPerPixel == 16) { imagePalette = new byte[3 * 4]; }
            else { imagePalette = new byte[0]; }
        }

        public Color[] getPalette()
        {
            Color[] palette = new Color[imagePalette.Length / 4];
            Color RGB;
            for (int i = 0; i < imagePalette.Length / 4; i++)
            {
                RGB = Color.FromArgb(imagePalette[i * 4 + 2], imagePalette[i * 4 +1],imagePalette[i * 4]);
                palette.SetValue(RGB, i);
            }
            return palette;
        }
    }
    class BDecodeFile
    {
        BReadHeader BMP_header = new BReadHeader();
        BReadPalette BMP_palette = new BReadPalette();
        public Bitmap decode_image;
        byte[] pixeldata;
        int datalen;
        int width;
        public void decoPixel(byte[] imagefile)
        {
            BMP_header.readheader(imagefile);
            BMP_palette.readpalette(imagefile);
            datalen = 14 + BMP_header.InfoHeaderSize + BMP_palette.imagePalette.Length;
            pixeldata = new byte[imagefile.Length - datalen]; //pixel data total (imagefile.Length - datalen) bytes
            Array.Copy(imagefile, datalen, pixeldata, 0, pixeldata.Length);
            width = (BMP_header.BitWidth+3)/4 * 4;
            decode_image = new Bitmap(width, BMP_header.BitHeight);
            if (BMP_header.Compression == 0)
            {
                
                if (BMP_header.BitsPerPixel == 32 || BMP_header.BitsPerPixel == 16)
                {
                    PixelFormat pixelformat = PixelFormat.Format16bppRgb555;
                    decode_image = new Bitmap(width, BMP_header.BitHeight, pixelformat);
                    decode_image = biRGB(pixeldata);
                }
                else { decode_image = biRGB(pixeldata); }
            }
            else if (BMP_header.Compression == 1)
            {
                decode_image = unpackRLE8(pixeldata);
            }
            else if (BMP_header.Compression == 2)
            {
                decode_image = unpackRLE4(pixeldata);
            }
            
        }
        public Bitmap biRGB(byte[] pixeldata) 
        {
            int pindex = 0;
            Bitmap imagepixel = new Bitmap(width, BMP_header.BitHeight);
            Color RGB;
            Color[] pixelcolor = new Color[width * (BMP_header.BitsPerPixel / 8) * BMP_header.BitHeight];
            if(BMP_header.BitsPerPixel == 32 || BMP_header.BitsPerPixel == 16) 
            {
                PixelFormat pixelformat = PixelFormat.Format16bppRgb555;
                imagepixel = new Bitmap(width, BMP_header.BitHeight, pixelformat);
            }
            for (int i = 0; i < pixelcolor.Length / (BMP_header.BitsPerPixel / 8); i++)
            {
                RGB = Color.FromArgb(pixeldata[i * 3 + 2], pixeldata[i * 3 + 1], pixeldata[i * 3]);
                pixelcolor.SetValue(RGB, i);
            }
            for (int m = BMP_header.BitHeight - 1; m >= 0; m--)
            {
                for (int n = 0; n < width; n++)
                {
                    imagepixel.SetPixel(n, m, pixelcolor[pindex]);
                    pindex++;
                }
            }
            return imagepixel;
        }
        public Bitmap unpackRLE8(byte[] pixeldata)
        {
            int pindex = 0;
            int readindex = 0;
            int writeindex = 0;
            int count;
            int size = BMP_header.BitHeight * width;
            byte[] pixelvalue = new byte[size];
            while (readindex < pixeldata.Length)
            {
                count = pixeldata[readindex];
                readindex++;
                byte bytevalue = pixeldata[readindex];
                if (count == 0x00) {
                    if (bytevalue == 0x00);
                    else if (bytevalue == 0x01)
                        break;
                    else if (bytevalue == 0x02)
                    {
                        readindex++;
                        int hoffset = pixeldata[readindex];
                        readindex++;
                        int voffset = pixeldata[readindex];
                        writeindex += hoffset;
                        writeindex += width * voffset;
                        writeindex++;
                    }
                    else if (bytevalue >= 0x03 && bytevalue <= 0xff)
                    {
                        int offset = (bytevalue + 1) / 2 * 2;
                        for (int i = 0; i < bytevalue; i++)
                        {
                            pixelvalue[writeindex] = pixeldata[readindex + 1 + i];
                            writeindex++;
                        }
                        readindex += offset;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        pixelvalue[writeindex+i] = bytevalue;
                    }
                    writeindex+=count ;
                }
                readindex++;
            }
            Bitmap imagepixel = new Bitmap(width, BMP_header.BitHeight);
            Color[] palette = BMP_palette.getPalette();
            for (int m = BMP_header.BitHeight - 1; m >=0; m--)
            {
                for (int n = 0; n < width; n++)
                {
                    imagepixel.SetPixel(n, m, palette[pixelvalue[pindex]]);
                    pindex++;
                }
            }
            return imagepixel;
        }
        public Bitmap unpackRLE4(byte[] pixeldata)
        {
            int pindex = 0;
            int readindex = 0;
            int writeindex = 0;
            int count;
            int size = BMP_header.BitHeight * width;
            byte[] pixelvalue = new byte[size];
            while (readindex < pixeldata.Length)
            {
                count = pixeldata[readindex];
                readindex++;
                byte bytevalue = pixeldata[readindex];
                byte bytevalue_0 = Convert.ToByte((bytevalue & 0xF0) >>4);
                byte bytevalue_1 = Convert.ToByte((bytevalue & 0x0F));
                if (count == 0) 
                {
                    if (bytevalue_0 == 0 && bytevalue_1 == 0);
                    else if (bytevalue_0 == 0 && bytevalue_1 == 1)
                        break;
                    else if (bytevalue_0 == 0 && bytevalue_1 == 2)
                    {
                        readindex++;
                        int hoffset = pixeldata[readindex];
                        readindex++;
                        int voffset = pixeldata[readindex];
                        writeindex += hoffset;
                        writeindex += width * voffset;
                        writeindex++;

                    }
                    else
                    {
                        int offset = (bytevalue + 3) / 4 * 2;
                        while(true)
                        {
                            count = bytevalue;
                            int index = readindex;
                            byte value = pixeldata[++index];
                            pixelvalue[writeindex] = Convert.ToByte((value & 0xF0) >> 4);
                            writeindex++;
                            count--;
                            if (count == 0) { break; }
                            pixelvalue[writeindex] = Convert.ToByte(value & 0x0F);
                            writeindex++;
                            count--;
                            if (count == 0) { break; }
                        }
                        readindex += offset;
                    }
                }
                else
                {
                    while(true)
                    {
                        pixelvalue[writeindex] = bytevalue_0;
                        writeindex++;
                        count--;
                        if (count == 0) { break; }
                        pixelvalue[writeindex] = bytevalue_1;
                        writeindex++;
                        count--;
                        if (count == 0) { break; }
                    }
                }
                readindex++;
            }
            Bitmap imagepixel = new Bitmap(width, BMP_header.BitHeight);
            Color[] palette = BMP_palette.getPalette();
            for (int m = BMP_header.BitHeight - 1; m >= 0; m--)
            {
                for (int n = 0; n < width; n++)
                {
                    imagepixel.SetPixel(n, m, palette[pixelvalue[pindex]]);
                    pindex++;
                }
            }
            return imagepixel;
        }
    }
}


