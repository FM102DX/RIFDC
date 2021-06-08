using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RIFDC;

namespace CoffeePointsDemo
{
    public partial class CoffeePointsFrm : Form
    {
        public CoffeePointsFrm()
        {
            InitializeComponent();
        }

        DataFormComponent dfc;

        private void CoffeePointsFrm_Load(object sender, EventArgs e)
        {
            ItemKeeper<CoffeePoint> coffeePointsDataSource = ItemKeeper<CoffeePoint>.getInstance(CoffeePointsApp.mainDataRoom);
            
            dfc = new DataFormComponent(coffeePointsDataSource, this, Lib.FrmCrudModeEnum.GridAndFieldsOnTheFly);

            //маппинг контролов
            //грид

            RIFDC_DataGridView grd0 = new RIFDC_DataGridView(
                                                            dgCoffeePoints, 
                                                            coffeePointsDataSource, 
                                                            CoffeePoint.MyControlFormats.ShortCoffeePointGridFormat.getMyInstance(coffeePointsDataSource.sampleObject), 
                                                            DataGridEditabilityMode.NotEditableAtAll);
            dfc.addGridBasedControlMapping(grd0);

            //поля
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbId), "id");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbCoffeePointName), "name");
            dfc.addRecordBasedControlMapping(new RIFDC_MaskedTextBox  (tbLastVisitDate), "lastVisitDate");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbBigLattePrice), "bigLattePrice");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbComment), "description");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbAlias), "alias");

            dfc.addRecordBasedControlMapping(new RIFDC_MaskedTextBox(tbLastVisitDate2), "lastVisitDate2");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbBigLattePrice2), "bigLattePrice2");
            dfc.addRecordBasedControlMapping(new RIFDC_TextBox(tbComment2), "description2");

            dfc.searchControl = new DFCSearchControl(dfc, Lib.FiltrationTypeEnum.Local, tbSearch, btnSearch, btnCancelSearch);

            //кнопки
            dfc.addButtonMapping(new RIFDC_Button(btnAddNewRecord, FormBtnTypeEnum.btnAddNew));
            dfc.addButtonMapping(new RIFDC_Button(btnDelete, FormBtnTypeEnum.btnDelete));
            dfc.addButtonMapping(new RIFDC_Button(btnSave, FormBtnTypeEnum.btnSelectAll));
            dfc.addButtonMapping(new RIFDC_Button(btnPrevious, FormBtnTypeEnum.btnRecordMovePrevRecord));
            dfc.addButtonMapping(new RIFDC_Button(btnNext, FormBtnTypeEnum.btnRecordMoveNextRecord));
            dfc.addButtonMapping(new RIFDC_Button(btnReload, FormBtnTypeEnum.btnReloadDataFormComponent));

            //dfc.addButtonMapping(new RIFDC_Button(btnGroupOperations, FormBtnTypeEnum.btnOpenGroupOperationsForm));
            dfc.addButtonMapping(new RIFDC_Button(btnHistory, FormBtnTypeEnum.btnOpenHistoryForm));

            dfc.crudOperator.addButtonMapping(new RIFDC_Button(btnToggleSpecLineMultiSelect, FormBtnTypeEnum.btnToggleMultiSelectionMode));
            dfc.crudOperator.addButtonMapping(new RIFDC_Button(btnSpecLineSelectAll, FormBtnTypeEnum.btnSelectAll));
            dfc.crudOperator.addButtonMapping(new RIFDC_Button(btnSpecLineSelectNone, FormBtnTypeEnum.btnSelectNone));


            //запуск
            dfc.readItems();
            dfc.fillTheForm();

        }
    }
}
