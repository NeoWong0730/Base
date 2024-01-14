using System;

namespace Framework
{
    public enum EUIState
    {
        Invalid,
        WaitShow,
        Showing,
        Show,
        WaitHide,
        Hiding,
        Hide,
        Close,
        Destroy,
    }

    [Flags]
    public enum EUIOption
    { 
        eInvalid = 0,
        eHideMainCamera = 1,            //UI�򿪺�ر���������� �����붯��������ɺ�
        eHideBeforeUI = 2,              //UI�򿪺�����ǰ��򿪵�UI ����ջ�е����ڵ㣩
        eIgnoreClear = 4,               //����UIManager.ClearUI����
        eIgnoreStack = 8,               //������UIջ�߼�����Ҫ�Զ���㼶sortingOrder
        eReduceFrameRate = 16,          //��UI�����ϲ��ʱ�ѵ�֡��ִ��
        eReduceMainCameraQuality = 32,  //��UI��ʾʱ���ͳ�������
        eDontHideFlag = 64,             //���UI������һ�������eDontHideBefore����UI����
        eDontHideBeforeIfHasFlag = 128, //������ǰһ����eDontHideFlag��ǵ���UI��������UI���ã�
    }

    public class UIConfigData
    {
        public readonly string prefabPath;
        public readonly Type script;
        public readonly EUIOption options;
        public readonly int order;

        public UIConfigData(Type script, string prefabPath, EUIOption options = EUIOption.eInvalid, int order = 0)
        {
            this.prefabPath = prefabPath;
            this.options = options;
            this.script = script;
            this.order = order;
        }
    }
}
