﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gemini.Modules.ErrorList.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Gemini.Modules.ErrorList.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 Column 的本地化字符串。
        /// </summary>
        public static string ErrorListHeaderColumn {
            get {
                return ResourceManager.GetString("ErrorListHeaderColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Description 的本地化字符串。
        /// </summary>
        public static string ErrorListHeaderDescription {
            get {
                return ResourceManager.GetString("ErrorListHeaderDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 File 的本地化字符串。
        /// </summary>
        public static string ErrorListHeaderFile {
            get {
                return ResourceManager.GetString("ErrorListHeaderFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Line 的本地化字符串。
        /// </summary>
        public static string ErrorListHeaderLine {
            get {
                return ResourceManager.GetString("ErrorListHeaderLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Errors 的本地化字符串。
        /// </summary>
        public static string ErrorTextPlural {
            get {
                return ResourceManager.GetString("ErrorTextPlural", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Error 的本地化字符串。
        /// </summary>
        public static string ErrorTextSingular {
            get {
                return ResourceManager.GetString("ErrorTextSingular", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Messages 的本地化字符串。
        /// </summary>
        public static string MessageTextPlural {
            get {
                return ResourceManager.GetString("MessageTextPlural", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Message 的本地化字符串。
        /// </summary>
        public static string MessageTextSingular {
            get {
                return ResourceManager.GetString("MessageTextSingular", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Error L_ist 的本地化字符串。
        /// </summary>
        public static string ViewErrorListCommandText {
            get {
                return ResourceManager.GetString("ViewErrorListCommandText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Error List 的本地化字符串。
        /// </summary>
        public static string ViewErrorListCommandToolTip {
            get {
                return ResourceManager.GetString("ViewErrorListCommandToolTip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Warnings 的本地化字符串。
        /// </summary>
        public static string WarningTextPlural {
            get {
                return ResourceManager.GetString("WarningTextPlural", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {0} Warning 的本地化字符串。
        /// </summary>
        public static string WarningTextSingular {
            get {
                return ResourceManager.GetString("WarningTextSingular", resourceCulture);
            }
        }
    }
}
