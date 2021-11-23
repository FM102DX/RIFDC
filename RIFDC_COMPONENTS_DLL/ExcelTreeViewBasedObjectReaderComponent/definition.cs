using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RIFDC;
using CommonFunctions;

namespace RIFDCComponents.ExcelTreeViewBasedObjectReaderComponent
{
    //тут класс, который управляет этим контролом

    public class ExcelTreeViewBasedObjectReaderComponent
    {
        private ExcelTreeViewBasedObjectReaderComponent()
        {

        }

        IKeeper _targetKeeper;
        Lib.getExcelSTDCInstance _driverDlg;
        Lib.IControlFormat _gridFormat;
        Form _parentForm;


        public static ExcelTreeViewBasedObjectReaderComponent getInstance(IKeeper targetKeeper, Lib.getExcelSTDCInstance driverDlg, Lib.IControlFormat gridFormat, Form parentForm)
        {
            ExcelTreeViewBasedObjectReaderComponent comp = new ExcelTreeViewBasedObjectReaderComponent();
            comp._targetKeeper = targetKeeper;
            comp._driverDlg = driverDlg; // это делегат, который возвращает драйвер чтения деревьев, прописанный в бизнес-объекте, т.к. сам драйвер заранее не создашь
            comp._gridFormat = gridFormat;
            comp._parentForm = parentForm;
            return comp;
        }

        public void run()
        {
            //ExcelObjectReaderFrm docFrm = new ExcelObjectReaderFrm();
            Lib.InterFormMessage msg = new Lib.InterFormMessage();
            //что вообще надо передать?
            // 1) тип объекта, который мы читаем
            // 2) какие-то стартовые фильтры, чтобы была понятна предустановка

            msg.sampleObject = _targetKeeper.sampleObject;
            msg.excelStaticDelegate = _driverDlg; // драйвер
            msg.targetKeeper = _targetKeeper;
            msg.controlFormat = _gridFormat; // контролформат

            msg.dataRoom = RIFDC_App.mainDataRoom; //TODO тут будет другой датарум

            ExcelTreeViewBasedObjectReader docFrm = new ExcelTreeViewBasedObjectReader();

            docFrm.startMsg = msg;

            docFrm.MdiParent = _parentForm;

            docFrm.Show();
        }
    }
}
