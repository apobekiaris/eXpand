﻿using System;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.Dashboard {
    public static class Extensions {
        public static string GetDashboardXml(this DevExpress.DashboardCommon.Dashboard dashboard){
            string xml;
            using (var ms = new MemoryStream()) {
                dashboard.SaveToXml(ms);
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    xml = sr.ReadToEnd();
                    sr.Close();
                }
                ms.Close();
            }
            return xml;

        }

        public static ProxyCollection CreateDashboardDataSource(this IObjectSpace objectSpace, Type objectType) {
            var proxyCollection = new ParameterLessProxyCollection(objectSpace, objectSpace.TypesInfo.FindTypeInfo(objectType), objectSpace.GetObjects(objectType));
            proxyCollection.DisplayableMembers = string.Join(";", proxyCollection.DisplayableMembers.Split(';').Where(s => !s.EndsWith("!")));
            return proxyCollection;
        }
    }

    public class ParameterLessProxyCollection:ProxyCollection{
        public ParameterLessProxyCollection(IObjectSpace objectSpace, ITypeInfo typeInfo, object collection) : base(objectSpace, typeInfo, collection){
        }
        public ParameterLessProxyCollection() : base(ObjectSpaceInMemory.CreateNew(), XafTypesInfo.Instance.FindTypeInfo(typeof(ParameterLessProxyCollection)), new object [0]){
        }

    }
}
