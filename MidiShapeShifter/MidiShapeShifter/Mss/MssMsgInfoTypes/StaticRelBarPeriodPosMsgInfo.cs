﻿namespace MidiShapeShifter.Mss.MssMsgInfoTypes
{
    public class StaticRelBarPeriodPosMsgInfo : StaticGenericRelitivePosMsgInfo
    {
        public override MssMsgType MsgType => MssMsgType.RelBarPeriodPos;

        public override string Data1Name => StaticMssMsgInfo.DATA_NAME_UNUSED;

        public override string Data2Name => StaticMssMsgInfo.DATA_NAME_UNUSED;

        public override string Data3Name => StaticMssMsgInfo.DATA3_NAME_PERIOD_POSITION;
    }
}
