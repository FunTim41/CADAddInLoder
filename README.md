

参照Revit SDK 里的那个AddinManager及网上给出的方法。通过反射。把dll复制到临时文件夹，然后委托调用。现在只做了CommandMethod部分的。VS2022+CAD2024。

![image-20250108201133703](https://github.com/user-attachments/assets/2456f16d-2771-4e15-bb4f-6c3225c14324)


使用方式：

1、直接安装，不要改安装地址。

2、可通过附加模块下的“插件管理”中的ribbon UI或者命令“AddInManager”进入。

