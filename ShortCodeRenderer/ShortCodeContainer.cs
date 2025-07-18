﻿using ShortCodeRenderer.Common;
using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Interfaces;
using ShortCodeRenderer.Common.Renderer;
using ShortCodeRenderer.Common.Tasks;
using ShortCodeRenderer.Importer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShortCodeRenderer
{
    public class ShortCodeContainer
    {

        public bool Contains(string name, bool searchInGlobalRenders = true) => (searchInGlobalRenders &&  ShortCodeGlobals.GlobalRenderers.ContainsKey(name)) || _renderers.ContainsKey(name);
        internal readonly Dictionary<string, IShortCodeRender> _renderers = new Dictionary<string, IShortCodeRender>(StringComparer.OrdinalIgnoreCase);
        public void AddRenderer(string name, string value)
        {
            _renderers[name] = new StringShortCodeRender(value);
        }
        public void AddRenderer(string name, Func<ShortCodeInfo, TaskOr<string>> value)
        {
            _renderers[name] = new FuncShortCodeRender(value);
        }
        public void AddRenderer(string name, IShortCodeRender renderer)
        {
            _renderers[name] = renderer;
        }
        public void ClearRenderers()
        {
            _renderers.Clear();
        }
        internal IShortCodeRender GetRenderer(string name, Dictionary<string, IShortCodeRender> tempRenderers)
        {
            if (tempRenderers != null && tempRenderers.TryGetValue(name, out var renderer))
                return renderer;
            if (_renderers.TryGetValue(name, out renderer))
                return renderer;
            if (ShortCodeGlobals.GlobalRenderers.TryGetValue(name, out renderer))
                return renderer;
            return null;
        }
        /// <summary>
        /// Cachelelemiş render'ları temizler. Bu, IShortCodeCache arayüzünü uygulayan render'lar için geçerlidir.
        /// </summary>
        public void FlushCachedRenderers(Dictionary<string, IShortCodeRender> tempRenderers = null)
        {
            foreach (var renderer in _renderers.Values)
            {
                if (renderer is IShortCodeCache cache)
                {
                    cache.Flush();
                }
            }
            foreach (var renderer in ShortCodeGlobals.GlobalRenderers.Values)
            {
                if (renderer is IShortCodeCache cache)
                {
                    cache.Flush();
                }
            }
            foreach (var renderer in tempRenderers?.Values ?? new Dictionary<string, IShortCodeRender>().Values)
            {
                if (renderer is IShortCodeCache cache)
                {
                    cache.Flush();
                }
            }
        }


        public void ImportRenderersFromAssembly(Assembly assembly, bool includeGlobal = true)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsAbstract || !typeof(IShortCodeRender).IsAssignableFrom(type))
                    continue;
                var renderer = (IShortCodeRender)Activator.CreateInstance(type);
                if (includeGlobal)
                {
                    ShortCodeGlobals.GlobalRenderers[type.Name] = renderer;
                }
                else
                {
                    _renderers[type.Name] = renderer;
                }
            }
        }
        public void ImportRenderersFromAssembly(string assemblyName, bool includeGlobal = true)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName), "Assembly name cannot be null or empty.");
            var assembly = Assembly.Load(assemblyName);
            ImportRenderersFromAssembly(assembly, includeGlobal);
        }
        private readonly object _lock = new object();
        public void RemoveSourceRenderers(string name)
        {
            lock (_lock)
            {
                RemoveSourceRenderersInternal(name);
            }
        }
        private void RemoveSourceRenderersInternal(string name)
        {
            List<string> toRemove = new List<string>();
            foreach (var item in _renderers)
            {
                if (item.Value is ShortCodeRenderBase renderBase && renderBase.Source == name)
                {
                    toRemove.Add(item.Key);
                }
            }
            foreach (var remove in toRemove)
            {
                _renderers.Remove(remove);
            }
        }
        public void ImportRenderersFromJsonText(string json)
        {
            var shortCodes = JsonSerializer.Deserialize<ShortCodeImport>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                ReadCommentHandling = JsonCommentHandling.Skip,
            });
            if(shortCodes == null || shortCodes.ShortCodes == null || shortCodes.ShortCodes.Count == 0)
                return;
            foreach (var shortCode in shortCodes.ShortCodes)
            {
                var typeDef  = ShortCodeGlobals.TypeDef.TryGetValue(shortCode.Type, out var func) ? func : null;
                if (typeDef == null)
                    continue;
                _renderers[shortCode.Name] = typeDef(shortCode.Value);
            }

        }
        public void ImportRenderersFromJsonFile(string filePath, bool watch = false, Action<Exception> exceptionHandler = null)
        {
            void __importFromFile()
            {
                lock (_lock)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            ImportRenderersFromJsonText(reader.ReadToEnd());
                        }
                    }
                }
            }
            void _importFromFile()
            {
                if (exceptionHandler == null)
                    __importFromFile();
                else
                {
                    try
                    {
                        __importFromFile();
                    }
                    catch (Exception ex)
                    {

                        exceptionHandler.Invoke(ex);
                    }
                }


            }
            if (watch)
            {
                FileInfo info = new FileInfo(filePath);
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = info.DirectoryName;
                watcher.Filter = Path.GetFileName(filePath);
                watcher.InternalBufferSize = 64 * 1024;
                watcher.Changed += (sender, e) =>
                {
                    lock (_lock)
                    {
                        RemoveSourceRenderersInternal(filePath);
                        _importFromFile();
                    }
                };
                watcher.Error += (sender, e) =>
                {
                 
                };
                watcher.Renamed += (sender, e) =>
                {
                    lock (_lock)
                    {
                        RemoveSourceRenderersInternal(filePath);
                        if(e.FullPath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                        {
                            _importFromFile();
                        }
                    }
                };
                watcher.Deleted += (sender, e) =>
                {
                    lock (_lock)
                    {
                        RemoveSourceRenderersInternal(filePath);
                    }
                };
                watcher.NotifyFilter = NotifyFilters.Attributes
                                                 | NotifyFilters.CreationTime
                                                 | NotifyFilters.DirectoryName
                                                 | NotifyFilters.FileName
                                                 | NotifyFilters.LastAccess
                                                 | NotifyFilters.LastWrite
                                                 | NotifyFilters.Security
                                                 | NotifyFilters.Size;
                watcher.EnableRaisingEvents = true;
            }
            lock (_lock)
            {
                _importFromFile();
            }

        }

    }
}
