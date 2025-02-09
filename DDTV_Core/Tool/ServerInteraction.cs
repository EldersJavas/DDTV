﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDTV_Core.Tool
{
    public class ServerInteraction
    {
        public class CheckUpdates
        {
            private static bool IsChecked = false;
            private static bool Is = false;
            public static void Update(string Type)
            {
                if (!Is)
                {
                    Task.Run(() =>
                    {
                        Is = true;
                        while (true)
                        {
                            try
                            {
                                if (IsChecked)
                                {
                                    Console.WriteLine("检测到新版本，请退出本程序后运行目录中的[DDTV_Update]进行更新:\nWindows请退出后直接运行DDTV_Update.exe\nLinux环境请使用dotnet DDTV_Update.dll运行更新程序[10分钟后再次提示]");  
                                }
                                else
                                {
                                    if (DDTV_Update.ComparisonVersion(Type, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                                    {
                                        IsChecked = true;
                                        Console.WriteLine("检测到新版本，请退出本程序后运行目录中的[DDTV_Update]进行更新:\nWindows请退出后直接运行DDTV_Update.exe\nLinux环境请使用dotnet DDTV_Update.dll运行更新程序[10分钟后再次提示]");
                                    }
                                }  
                            }
                            catch (Exception)
                            {
                            }
                            Thread.Sleep(60 * 30 * 1000);
                        }
                    });
                }
            }
        }

        public class Dokidoki
        {
            private static bool Is = false;
            private static int i = 0;
            public static void Start(string Type)
            {
               if(!Is)
                {
                    Is = true;
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            try
                            {
                                DDTV_Core.Tool.Dokidoki.SendDokidoki(Type, i.ToString());
                                i++;
                                Thread.Sleep(3600 * 1000);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    });
                }
            }
        }
    }
}
