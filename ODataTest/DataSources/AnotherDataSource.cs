// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.OData;
using Microsoft.OData.Edm;

namespace DynamicEdmModelCreation.DataSource
{
    internal class AnotherDataSource : IDataSource
    {
        private IEdmEntityType _enumTable;

        public void GetModel(EdmModel model, EdmEntityContainer container)
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsEnum && t.Namespace == "ODataTest"
                    select t;

            foreach (var item in q)
            {
                EdmEntityType enumTable = new EdmEntityType("ODataTest", item.Name);
                enumTable.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
                var key = enumTable.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                enumTable.AddKeys(key);
                model.AddElement(enumTable);
                EdmEntitySet students = container.AddEntitySet($"{item.Name}s", enumTable);
            }

        }

        public void Get(IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection)
        {
            var enumType = (from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.IsEnum && t.FullName == entityType.FullName()
                            select t).Single();

            foreach (int val in Enum.GetValues(enumType))
            {
                EdmEntityObject entity = new EdmEntityObject(entityType);
                entity.TrySetPropertyValue("Title", Enum.GetName(enumType, val));
                entity.TrySetPropertyValue("Id", val);
                collection.Add(entity);
            }
        }

        public void Get(string key, EdmEntityObject entity)
        {
            var enumType = (from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.IsEnum && t.FullName == entity.ActualEdmType.FullTypeName()
                            select t).Single();

            var value = int.Parse(key);

            entity.TrySetPropertyValue("Title", Enum.GetName(enumType, value));
            entity.TrySetPropertyValue("Id", value);
        }

        public object GetProperty(string property, EdmEntityObject entity)
        {
            object value;
            entity.TryGetPropertyValue(property, out value);
            return value;
        }

        private IEdmEntityObject Createchool(int id, DateTimeOffset dto, IEdmStructuredType edmType)
        {
            IEdmNavigationProperty navigationProperty = edmType.DeclaredProperties.OfType<EdmNavigationProperty>().FirstOrDefault(e => e.Name == "School");
            if (navigationProperty == null)
            {
                return null;
            }

            EdmEntityObject entity = new EdmEntityObject(navigationProperty.ToEntityType());
            entity.TrySetPropertyValue("ID", id);
            entity.TrySetPropertyValue("CreatedDay", dto);
            return entity;
        }
    }
}
