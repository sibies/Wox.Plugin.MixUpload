using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Wox.Plugin.MixUpload
{
    public class Main : IPlugin, IPluginI18n, ISettingProvider
    {

        private PluginInitContext _context;
        private MixUploadClient _client;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _client = new MixUploadClient(_context);
        }

        public List<Result> Query(Query query)
        {
            if (query.FirstSearch.Length > 0)
            {
                return _client.Search(query.Search);
            }
            return new List<Result>();
        }

        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("wox_plugin_mixupload_plugin_name");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("wox_plugin_mixupload_plugin_description");
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

    }
}
