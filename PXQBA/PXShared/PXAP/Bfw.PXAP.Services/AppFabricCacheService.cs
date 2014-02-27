using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PXAP.Components;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PXAP.Services
{
    public class AppFabricCacheService
    {
        private ICacheProvider cacheProvider;
        private readonly string defaultRegion;
        private const string NO_DATA = "NO DATA";

        public AppFabricCacheService()
        {
            cacheProvider = ServiceLocator.Current.GetInstance<ICacheProvider>();
            defaultRegion = ConfigurationManager.AppSettings["CacheRegionName"];
        }

        public string GetFromCache(string key, string region, out string tagsResult)
        {
            tagsResult = null;
            region = GetRegion(region);
            var item = cacheProvider.Fetch(key, region);
            if (item != null)
            {
                tagsResult = GetItemTagsXml(region, key);
                string resultXml = "";
                try
                {
                    resultXml = GetObjectXmlRepresentation(item);
                }
                catch (Exception ex)
                {
                    var node = GetCacheItemNode(key);
                    node.Add(GetNotSerializedNode(item, ex));
                    resultXml = node.ToString();
                }
                return resultXml;
            }
            return NO_DATA;
        }

        public string GetByTags(string tags, string region, FindType type, out string tagsResult)
        {
            tagsResult = null;
            IDictionary<string, object> retrieved = null;
            region = GetRegion(region);
            if (type == FindType.Tag)
                retrieved = cacheProvider.FetchByTag(tags, region);
            else if (type == FindType.AnyTag)
                retrieved = cacheProvider.FetchByAnyTag(GetAsList(tags), region);
            else if (type == FindType.AllTags)
                retrieved = cacheProvider.FetchByAllTags(GetAsList(tags), region);
            if (retrieved != null)
            {
                var tasks = new List<Task<string>> { 
                    Task<string>.Factory.StartNew(() => GetItemsFromTagQuery(retrieved)),
                    Task<string>.Factory.StartNew(() => GetItemTagsXml(region, retrieved.Keys.ToArray()))
                };
                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                tagsResult = tasks[1].Result;
                return tasks[0].Result;
            }
            return NO_DATA;
        }

        public bool RemoveByKey(string key, string region)
        {
            region = GetRegion(region);
            var objectCached = cacheProvider.Remove(key, region);
            return objectCached != null;
        }

        public bool RemoveByTag(string tag, string region)
        {
            region = GetRegion(region);
            var objectDictionary = cacheProvider.RemoveByTag(tag, region);
            return objectDictionary != null && objectDictionary.Values.Count > 0;
        }

        private string GetItemXml(Item item)
        {
            var entity = item.ToEntity();
            var xml = entity.ToString();
            return !String.IsNullOrEmpty(xml) ? xml : NO_DATA;
        }

        private string GetSerialized(object item)
        {
            var itemString = item as String;
            if (itemString != null)
                return itemString;
            var xnode = item as XNode;
            if (xnode != null)
                return xnode.ToString();
            var xmlNode = item as XmlNode;
            if (xmlNode != null)
                return GetXmlNode(xmlNode);
            //finally just serialize object to Xml
            return SerializeToXml(item);
        }

        private string GetXmlNode(XmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                node.WriteTo(writer);
            }
            return sb.ToString();
        }

        private string SerializeToXml(object item)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                DataContractSerializer serializer = new DataContractSerializer(item.GetType());
                serializer.WriteObject(writer, item);
            }
            return sb.ToString();
        }

        private string GetRegion(string region)
        {
            return String.IsNullOrEmpty(region) ? defaultRegion : region;
        }

        private List<string> GetAsList(string tags)
        {
            var tagArray = tags.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return tagArray.ToList<string>();
        }

        private string GetObjectXmlRepresentation(object item)
        {
            var contentItem = item as ContentItem;
            Item agilixItem = null;
            if (contentItem != null)
            {
                agilixItem = contentItem.ToItem();
            }
            else
            {
                agilixItem = item as Item;
            }
            if (agilixItem != null)
            {
                var result = GetItemXml(agilixItem);
                return result;
            }
            return GetSerialized(item);
        }

        private XElement GetTagsForItemNode(string key, string region)
        {
            var root = new XElement("tags");
            var tags = cacheProvider.GetCacheItemTags(key, region);
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    root.Add(new XElement("tag", tag));
                }
            }
            return root;
        }

        private string GetItemsFromTagQuery(IDictionary<string, object> objects)
        {
            XElement root = new XElement("items");
            foreach (KeyValuePair<string, object> kvp in objects)
            {
                var keyNode = GetCacheItemNode(kvp.Key);
                root.Add(keyNode);
                try
                {
                    var node = GetObjectXmlRepresentation(kvp.Value);
                    keyNode.Add(XElement.Parse(node));
                }
                catch (Exception ex)
                {
                    keyNode.Add(GetNotSerializedNode(kvp.Value, ex));
                }
            }
            return root.HasElements ? root.ToString() : NO_DATA;
        }

        private string GetItemTagsXml(string region, params string[] keys)
        {
            XElement root = new XElement("items");
            foreach (var key in keys)
            {
                var keyNode = GetCacheItemNode(key);
                root.Add(keyNode);
                keyNode.Add(GetTagsForItemNode(key, region));
            }
            return root.ToString();
        }

        private XElement GetCacheItemNode(string key)
        {
            return new XElement("cacheItem", new XAttribute("key",key));
        }

        private XElement GetNotSerializedNode(object item, Exception ex)
        {
            return new XElement("NotSerializedItem",
                        new XElement("type", item.GetType().FullName),
                        new XElement("exception", ex.Message)
            );
        }
    }
}
