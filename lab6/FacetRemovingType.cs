using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public enum FacetRemovingType
    {
        None, ZBuffer, BackfaceCulling
    }

    public static class FacetRemovingMethods
    {
        public static string GetFacetRemovingName(this FacetRemovingType facetRemovingType)
        {
            switch (facetRemovingType)
            {
                case FacetRemovingType.None:
                    return "Без отсечения (рисование ребер)";
                case FacetRemovingType.ZBuffer:
                    return "Z-буфер";              
                case FacetRemovingType.BackfaceCulling:
                    return "Отчесение по нормалям поверхностей";
                default:
                    throw new ArgumentException("Неизвестный тип отсечения граней");
            }
        }
    }
}
