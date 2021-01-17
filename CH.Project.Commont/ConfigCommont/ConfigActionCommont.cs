using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;


namespace CH.Project.Commont.ConfigCommont
{
    public class ConfigActionCommont : SingleCommont<ConfigActionCommont>
    {
        private static IConfigurationBuilder builder { get; set; }

        public ConfigActionCommont()
        {
            //builder =new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void AddJsonFile(string fileName)
        {
            //builder = builder.AddJsonFile(fileName, true, reloadOnChange: true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            var config = builder.Build();
            string value = string.Empty;
            foreach (var provider in config.Providers)
            {
                if (provider.TryGet(key, out value))
                {
                    return value;
                }
            }
            return value;
        }

        //public static IConfigurationBuilder AddJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        //{
        //    if (builder == null)
        //    {
        //        throw new ArgumentNullException(nameof(builder));
        //    }
        //    if (string.IsNullOrEmpty(path))
        //    {
        //        throw new ArgumentException(SR.Error_InvalidFilePath, nameof(path));
        //    }

        //    return builder.AddJsonFile(s =>
        //    {
        //        s.FileProvider = provider;
        //        s.Path = path;
        //        s.Optional = optional;
        //        s.ReloadOnChange = reloadOnChange;
        //        s.ResolveFileProvider();
        //    });
        //}
    }
}
