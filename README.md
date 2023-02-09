nsysu_FileReader 
==========================
# 使用介面
![image](https://user-images.githubusercontent.com/52366230/217748905-5c5396db-26fa-4612-aaa1-908c27252a75.png)

# 功能介紹
Open: PCX、BMP檔，會顯示header以及色盤

RGB: 顯示RGB channel

HSI: 顯示HSI

Cut: 可切取方形或是橢圓範圍

Brightness: 改變明亮度

Negitave: 顯示負片影像

Histogram: 顯示原圖與灰階的histogram，以及equalization、specific histogram

Rotate: 旋轉影像，可選正向失真映射與反向映射

Overlap: 開啟第二張影像重疊至第一張，並控制透明度

Graylevel: 顯示灰階影像

Constract: 控制對比度

Binarization: 設定threshold產生二值化影像，或是計算Otsu thresholding顯示2值化影像

Slicing: 顯示影像的bit-plane，以及可以開啟圖檔設定浮水印於指定的bit-Plane

Connected component: 顯示處理後的影像，以不同顏色標示不同相連區域

縮放:輸入指定倍率分別對寬或高縮放，使用Average/Interpolation 或 Decimation/Duplication

Filter:

![image](https://user-images.githubusercontent.com/52366230/217756527-356e41e7-239d-4187-a2b6-8310ee18425f.png)

可輸入指定kernel size 使用以下操作

Edge crispening: 可輸入指定kernel size

Outflier、Lowpass、Highpass、Median(square、cross)、Pseudo Median

High boost、Roberts、Sobel、Prewitt

Mpeg:

讀取tiff影像並進行播放、暫停、往前往後等操作

計算各個pixel的motion vector
