using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIFDC
{
   public static class WindowsServiceFucntions
    {
        // messageboxses
        public static void mb_info(string txt)
        {
            MessageBox.Show(txt, "Сообщение", MessageBoxButtons.OK);
        }
        public static void mb_info(double val)
        {
            MessageBox.Show(val.ToString(), "Сообщение", MessageBoxButtons.OK);
        }
        public static void mb_criticalError(string txt)
        {
            MessageBox.Show(txt.ToString(), "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool mb_confirmAction(string msg)
        {
            if (MessageBox.Show(msg, "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}
