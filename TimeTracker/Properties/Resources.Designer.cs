﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimeTracker.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TimeTracker.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data Source={0};Version=3;.
        /// </summary>
        internal static string connection_string {
            get {
                return ResourceManager.GetString("connection_string", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Script Date: 12/23/2017 9:30 AM  - ErikEJ.SqlCeScripting version 3.5.2.74
        ///DROP TABLE IF EXISTS [Timer];
        ///CREATE TABLE [Timer] (
        ///  [Id] INTEGER PRIMARY KEY AUTOINCREMENT
        ///, [Name] TEXT NULL UNIQUE
        ///, [Elapsed] TEXT DEFAULT &quot;00:00:00&quot; NULL
        ///);
        ///
        ///INSERT INTO [Timer] VALUES(null,&quot;One&quot;,&quot;00:01:00&quot;);
        ///INSERT INTO [Timer] VALUES(null,&quot;Two&quot;,&quot;00:02:00&quot;);
        ///INSERT INTO [Timer] VALUES(null,&quot;Three&quot;,&quot;00:03:00&quot;);
        ///INSERT INTO [Timer] VALUES(null,&quot;Four&quot;,&quot;00:04:00&quot;);
        ///INSERT INTO [Timer] VALUES(null,&quot;Five&quot;,&quot;00:05:00&quot;); [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string dbo_CreateTimer {
            get {
                return ResourceManager.GetString("dbo_CreateTimer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM [Timer] WHERE Id = @Id;.
        /// </summary>
        internal static string delete_command {
            get {
                return ResourceManager.GetString("delete_command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to insert into [Timer] values(null, @Name, @Elapsed);.
        /// </summary>
        internal static string insert_command {
            get {
                return ResourceManager.GetString("insert_command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT Name FROM [Timer] WHERE Name = @Name;.
        /// </summary>
        internal static string name_exists_command {
            get {
                return ResourceManager.GetString("name_exists_command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Save {
            get {
                object obj = ResourceManager.GetObject("Save", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT id, Name, Elapsed FROM [Timer];.
        /// </summary>
        internal static string select_all_rows {
            get {
                return ResourceManager.GetString("select_all_rows", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE [Timer] SET Name = @Name, Elapsed = @Elapsed WHERE Name = @Name;.
        /// </summary>
        internal static string update_command {
            get {
                return ResourceManager.GetString("update_command", resourceCulture);
            }
        }
    }
}
