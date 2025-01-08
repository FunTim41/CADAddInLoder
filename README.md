每次调试cad插件，老是会有占用的问题。网上找到方法就是通过反射解决，但是只找到那个要自己手动配置的，嫌麻烦。GitHub上有个[CADAddInManager](https://github.com/chuongmep/CadAddinManager)，用了一下，好像没什么反应，不知道是不是使用方式不对，文档也不详细。索性自己整一个。

参照Revit SDK 里的那个AddinManager及网上给出的方法。通过反射。把dll复制到临时文件夹，然后委托调用。现在只做了CommandMethod部分的。VS2022+CAD2024。

![image-20250108201133703](https://github.com/user-attachments/assets/2456f16d-2771-4e15-bb4f-6c3225c14324)


使用方式：

1、先自动加载：

在cad根目录Support文件夹下找到acad2024.lsp。

```
(command"netload" "Dll所在文件夹\\CADAddinManagerDemo.dll")
```

2、把“引用文件”文件夹下的内容复制到cad根目录中，启动CAD后用“InitAddin”进行初始化，成功后菜单栏会显示“插件管理”UI及显示上图界面。

3、下次运行可通过菜单栏添加“插件管理”UI或者“ShowAddInManager”进入。


