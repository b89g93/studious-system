using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateAssistant
{
    class UpdateInfoList
    {
        public static void AddItem(ListView listView,String item)
        {
            if(item == "")
            {
                MessageBox.Show("添加内容不能为空");
                return;
            }
            listView.Items.Add(item);
        }

        public static void DeleteSelectedItems(ListView listView)
        {
            foreach (ListViewItem deleteItem in listView.SelectedItems)
            {
                listView.Items.Remove(deleteItem);
            }
        }
    }
}
