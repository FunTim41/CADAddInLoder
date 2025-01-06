﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace CADAddinManagerDemo
{
    public class Command
    {
        [CommandMethod("AddInManagerByFT")]
        public void BuildMyPopMenu()
        {
            try
            {
                //自定义的组名
                string strMyGroupName = "AddInManagerByFT";
                //保存的CUI文件名（从CAD2010开始，后缀改为了cuix）
                //这里可以写文件路径或者仅写文件名
                //如果仅写文件名，则最后保存时将默认保存在此程序集DLL所在目录
                //如果写文件路径，则会按路径进行保存
                string strCuiFileName = "AddInManagerByFT.cui";

                //创建一个自定义组（这个组中将包含我们自定义的命令、菜单、工具栏、面板等）
                CustomizationSection myCSection = new CustomizationSection();
                myCSection.MenuGroupName = strMyGroupName;

                //创建自定义命令组
                MacroGroup mg = new MacroGroup("MyMethod", myCSection.MenuGroup);

                MenuMacro mm3 = new MenuMacro(mg, "进入管理", "ShowAddInManager", "");

                //声明菜单别名
                StringCollection scMyMenuAlias = new StringCollection(); //菜单别名（仅查看CUI源文件时能看到）
                scMyMenuAlias.Add("MyPop1");
                scMyMenuAlias.Add("MyTestPop");

                //菜单项（将显示在项部菜单栏中）
                PopMenu pmParent = new PopMenu(
                    "插件管理",
                    scMyMenuAlias,
                    "插件管理",
                    myCSection.MenuGroup
                );

                //子项的菜单（单级）
                PopMenuItem pmi3 = new PopMenuItem(
                    mm3,
                    "进入管理(&ShowAddInManager)",
                    pmParent,
                    -1
                );

                // 最后保存文件
                myCSection.SaveAs(strCuiFileName);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("自定义CAD菜单失败：" + ex.Message);
            }
        }

        [CommandMethod("ShowAddInManager")]
        public void ShowAddInManager()
        {
            MainView mainView = new MainView();
            mainView.Show();
        }
    }
}