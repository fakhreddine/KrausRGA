using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;


namespace KrausRGA.Views
{
   public static class Validation
    {
       /// <summary>
       /// validation  for combobox  
       /// </summary>
       /// <param name="cb"></param>
       /// <returns></returns>
       public static Boolean forcombobox(this ComboBox cm)
       {
           return cm.SelectedIndex != 0 ?  false: true;
       }

       //public static Boolean gridrow(this DataGrid dg)
       //{
       //    return dg.SelectedCells[3]==null ? true : false;
       //}
         
    }
}
