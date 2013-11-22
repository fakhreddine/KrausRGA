using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KrausRGA.Views
{
    public static class ExtensionParent
    {
        /// <summary>
        /// Avinash
        /// This is Extemtion method that gives the parent control as per request.
        ///This is the Recursive fuction call method.
        /// </summary>
        /// <typeparam name="T"> Generic value Parameter </typeparam>
        /// <param name="child">which controls parent we want to find</param>
        /// <returns> parent control object</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent is T)
                return parent as T;
            else
                return parent != null ? FindParent<T>(parent) : null;
        }

        /// <summary>
        /// Make Border as checked.
        /// </summary>
        /// <param name="Bdr">
        /// Border object.
        /// </param>
        public static void Inside(this Border Bdr)
        {
            Bdr.BorderThickness = new Thickness(2, 2, 4, 4);
        }

        /// <summary>
        /// Make Border as realesed from click.
        /// </summary>
        /// <param name="Bdr">
        /// Border Realesed.
        /// </param>
        public static void Outside(this Border Bdr)
        {
            Bdr.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }
}
