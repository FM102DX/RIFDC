using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonFunctions;

namespace RIFDC
{
    public partial class HistoryManagerFrm : Form, IRIFDCCrudForm
    {
        public HistoryManagerFrm()
        {
            InitializeComponent();
        }

        DataFormComponent HistoryManagerDFC;

        public Lib.InterFormMessage startMsg { get; set; } = null;

        private void HistoryManagerFrm_Load(object sender, EventArgs e)
        {
            // эта форма должна показывать все операции по этому объекту и давать делать Undo последней операции
            // TODO ведь удаление отмена этих изменений влияет на остальные объекты и как именно

            if (startMsg == null) return;

            //берем объект, который отвечает за хранение истории по типу объектов
            HistorySaver hs = HistorySaver.getInstance(RIFDC_App.mainDataRoom);

            IKeeper HistoryManagerDataSource = hs.getMyHistoryIKeeperObject(startMsg.targetObject);

            HistoryManagerDFC = new DataFormComponent(HistoryManagerDataSource, this, Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly);

            //грид
            //инициализация грида
            RIFDC_DataGridView grd0 = new RIFDC_DataGridView(dgObjectHistory, HistoryManagerDataSource,
                       HistorySaver.HistorySaverUnit.MyControlFormats.HistorySaverUnitControlFormat.getMyInstance(HistoryManagerDataSource.sampleObject));

            //маппинг грида
            HistoryManagerDFC.crudOperator.addGridBasedControlMapping(grd0);

            HistoryManagerDFC.fillTheForm();

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnCancelHistoryItem_Click(object sender, EventArgs e)
        {
            //отмена посденей операции
            if (HistoryManagerDFC.dataSource.count == 0) return;

            if (!ServiceFucntions.mb_confirmAction("Отменить последнюю операцию?")) return;
            
            IKeepable _hsUnit = HistoryManagerDFC.currentRecord.getMember();

            HistorySaver.HistorySaverUnit hsUnit=null;

            if (_hsUnit == null) return;

            try
            {
                hsUnit = (HistorySaver.HistorySaverUnit)_hsUnit;
            }
            catch
            {
                return;
            }

            HistorySaver hs = HistorySaver.getInstance(RIFDC_App.mainDataRoom);

           Lib.ObjectOperationResult or = hs.doRollbackOperation(hsUnit);
        }

        private void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (HistoryManagerDFC.dataSource.count == 0) return;
            if (!ServiceFucntions.mb_confirmAction("Очистить историю?")) return;
            HistoryManagerDFC.dataSource.deleteFiteredPackege(HistoryManagerDFC.dataSource.filtration.myActualGlobalFilter);
        }

        private void dgObjectHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
