using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticalCenter.DataAccess
{
    public class SystemDBConfig
    {
        /// <summary>
        /// DIY书籍审核表
        /// </summary>
        public static string T_DIY_Book_Review = "[magic_book].[dbo].[T_DIY_Book_Review]";

        public static string T_DIY_Book_Tag_List = "magic_book.dbo.T_DIY_Book_Tag_List";

        /// <summary>
        /// 已发布书籍表
        /// </summary>
        public static string Dream_Multimedia_Book = "[magic_book].[dbo].[dream_multimedia_book]";

        /// <summary>
        /// 图书编辑日志
        /// </summary>
        public static string dream_multimedia_book_editlog="[magic_book].[dbo].[dream_multimedia_book_editlog]";

        /// <summary>
        /// 图书编辑信息暂存表
        /// </summary>
        public static string dream_multimedia_book_edit = "[magic_book].[dbo].[dream_multimedia_book_edit]";

        /// <summary>
        /// 关键词对应的图书
        /// </summary>
        public static string T_KeyWord_Books = "magic_book.dbo.T_KeyWord_Books";

        /// <summary>
        /// 图书对应的关键词
        /// </summary>
        /// <returns></returns>
        public static string T_Book_KeyWords = "magic_book.dbo.T_Book_KeyWords";

        public static string T_BookVersionConfig = "magic_book.dbo.T_BookVersionConfig";

        /// <summary>
        /// 反馈表
        /// </summary>
        public static string Feed_Back = "[magic_book].[dbo].[feed_back]";

        /// <summary>
        /// 用户信息表
        /// </summary>
        public static string UserInfo = " [UserData].[dbo].[UserInfo]";

        public static string SendMsgToUser = "[UserData].[dbo].[SendMsgToUser]";

        /// <summary>
        /// 统计表
        /// </summary>
        public static string BookUGC_Count = "[magic_book].[dbo].[BookUGC_Count]";

        /// <summary>
        /// 统计表
        /// </summary>
        public static string download_book_count = "[magic_book].[dbo].[download_book_count]";

        /// <summary>
        /// 4D课件审核
        /// </summary>
        public static string T_CourseReview = "[mxr_cw].[dbo].[T_CourseReview]";
        /// <summary>
        /// 4D课件审核日志
        /// </summary>
        public static string T_CourseReviewLog = "[mxr_cw].[dbo].[T_CourseReviewLog]";

        /// <summary>
        /// 4D课件资源
        /// </summary>
        public static string T_CourseReviewResource = "[mxr_cw].[dbo].[T_CourseReviewResource]";

        /// <summary>
        /// 4D课件类型
        /// </summary>
        public static string T_CourseClass = "[mxr_cw].[dbo].[T_CourseClass]";

        /// <summary>
        /// 用户上传DIY书籍花费的梦想币
        /// </summary>
        public static string T_User_Upload_DIY_Book_Used_Coin_Log = "[magic_book].[dbo].[T_User_Upload_DIY_Book_Used_Coin_Log]";

        /// <summary>
        /// 出版社表
        /// </summary>
        public static string Press = "[magic_book].dbo.Press";

        public static string PressBanner = "magic_book.dbo.PressBanner";
        /// <summary>
        /// 梦想人员工负责的出版社
        /// </summary>
        public static string T_AdminPress = "[magic_book].dbo.T_AdminPress";

        /// <summary>
        /// 图书下载记录表
        /// </summary>
        public static string download_book = "[magic_book].dbo.download_book";

        /// <summary>
        /// 点击记录表
        /// </summary>
        public static string click_hotspot = "magic_book.dbo.click_hotspot";
        /// <summary>
        /// 点击统计表
        /// </summary>
        public static string click_hotspot_count = "magic_book.dbo.click_hotspot_count";

        public static string BookUGC = "magic_book.dbo.BookUGC";

        /// <summary>
        /// 梦想币申请表
        /// </summary>
        public static string MXBQuotaApply = "magic_book.dbo.MXBQuotaApply";

        public static string MXBCheck = "magic_book.dbo.MXBCheck";

        /// <summary>
        /// 梦想币消息审批表
        /// </summary>
        public static string QuotaApply = "magic_book.dbo.QuotaApply";

        public static string MXBTotalQuato = "[magic_book].[dbo].[MXBTotalQuato]";

        /// <summary>
        /// 评论与回复举报
        /// </summary>
        public static string T_BookCommentReport = "magic_book.dbo.T_BookCommentReport";

        /// <summary>
        /// 图书评论表
        /// </summary>
        public static string T_BookComment = "magic_book.dbo.T_BookComment";
        /// <summary>
        /// 图书评论回复表
        /// </summary>
        public static string T_BookCommentReply = "magic_book.dbo.T_BookCommentReply";

        /// <summary>
        /// 图书阅读明细
        /// </summary>
        public static string T_Book_ReadingDuration_Logs = "magic_book.dbo.T_Book_ReadingDuration_Logs";

        /// <summary>
        /// 图书阅读明细统计
        /// </summary>
        public static string T_Book_ReadingDuration_Logs_Count=" [magic_book].[dbo].[T_Book_ReadingDuration_Logs_Count]";

        /// <summary>
        /// 用户设备表
        /// </summary>
        public static string T_User_Device_List = "magic_book.dbo.T_User_Device_List ";

        /// <summary>
        /// 设备信息表
        /// </summary>
        public static string T_Device_Info = "magic_book.dbo.T_Device_Info";

        public static string T_Device_Update_APP_Log = "magic_book.dbo.T_Device_Update_APP_Log";

        /// <summary>
        /// 充值记录表
        /// </summary>
        public static string T_DeviceCoin_History = "magic_book.dbo.T_DeviceCoin_History";

        /// <summary>
        /// 用户阅读记录（付梦想币）
        /// </summary>
        public static string T_DeviceCoin_Read = "magic_book.dbo.T_DeviceCoin_Read";

        /// <summary>
        /// 用户购买记录（消费记录）
        /// </summary>
        public static string T_DevicePurchase_History = "magic_book.dbo.T_DevicePurchase_History";

        /// <summary>
        /// 用户购买记录日统计表
        /// </summary>
        public static string T_DevicePurchase_History_Count = "magic_book.dbo.T_DevicePurchase_History_Count";

        /// <summary>
        /// Banner点击记录
        /// </summary>
        public static string T_Device_ClickBanner = "magic_book.dbo.T_Device_ClickBanner";

        /// <summary>
        /// Banner点击统计表
        /// </summary>
        public static string T_Device_ClickBanner_Count = "magic_book.dbo.T_Device_ClickBanner_Count";

        /// <summary>
        /// Banner列表
        /// </summary>
        public static string TagBanner = "magic_book.dbo.TagBanner";

        public static string TagBanner_Temp = "[magic_book].[dbo].[TagBanner_Temp]";
        public static string TagInformation = "[magic_book].[dbo].TagInformation";

        public static string TagBanner_Temp_Audit = "[magic_book].[dbo].TagBanner_Temp_Audit]";
        /// <summary>
        /// 二维码表
        /// </summary>
        public static string T_BookAPKTwoCode = "[magic_book].[dbo].[T_BookAPKTwoCode]";

        public static string T_Customization_Info = "[magic_book].[dbo].T_Customization_Info";

        public static string CountToolHistory = "[book_store_data_collection].[dbo].[T_CountToolHistory]";
        /// <summary>
        /// 二维码对应的图书
        /// </summary>
        public static string T_BookApkTwoCodeBook = "[magic_book].[dbo].[T_BookApkTwoCodeBook]";

        public static string PageUrlConfig = "[magic_book].[dbo].[PageUrlConfig]";

        public static string T_User_Private_Message_List_Temp = "messagedata.dbo.T_User_Private_Message_List_Temp";

        public static string T_User_Private_Message_Content_Temp = "messagedata.dbo.T_User_Private_Message_Content_Temp";

        public static string T_User_Private_Message_User_List = "messagedata.dbo.T_User_Private_Message_User_List";

        public static string T_User_Private_Message_List = "messagedata.dbo.T_User_Private_Message_List";

        public static string T_User_Private_Message_Content = "messagedata.dbo.T_User_Private_Message_Content";

        public static string T_DIY_Book_Review_Log = "magic_book.[dbo].[T_DIY_Book_Review_Log]";

        public static string T_DeviceCoin_All = " magic_book.dbo.T_DeviceCoin_All ";

        public static string book_series = "magic_book.dbo.book_series";

        public static string age_group = " magic_book.dbo.age_group";

        public static string BooksUnShelveCheck = " magic_book.dbo.BooksUnShelveCheck";

        public static string TemplateInformation = " magic_book.dbo.TemplateInformation";

        public static string TemplateTagList = "magic_book.dbo.TemplateTagList";

        public static string Research_Temp = "[magic_book].[dbo].[Research_Temp]";

        public static string Research_Temp_Audit = "[magic_book].[dbo].Research_Temp_Audit";

        public static string Research = "[magic_book].[dbo].[Research]";

        public static string researchBookRelation_Temp = "[magic_book].[dbo].[researchBookRelation_Temp]";

        public static string T_Report_Book = "magic_book.dbo.T_Report_Book";

        public static string LockBookSeries = "magic_book.dbo.LockBookSeries";

        public static string LevelPropertyList = " magic_book.dbo.[LevelPropertyList]";

        public static string LevelInformation = "magic_book.dbo.[LevelInformation]";

        public static string BookPropertyList = "magic_book.dbo.BookPropertyList";

        public static string BookPropertyList_Temp = "magic_book.dbo.BookPropertyList_Temp";

        public static string PropertyInformation = "magic_book.dbo.PropertyInformation";

        public static string tabRecord = "magic_book.dbo.tabRecord";

        public static string T_LockedBookList = " magic_book.dbo.T_LockedBookList ";

        public static string T_LevelProperties = " magic_book.dbo.T_LevelProperties";

        public static string messageReply = "magic_book.dbo. messageReply";

        public static string UserMessage = "magic_book.dbo.UserMessage";

        public static string messageInfo_Magic_Book = "magic_book.dbo.messageInfo";

        public static string MessageContent_Magic_Book = "magic_book.dbo.MessageContent";

        public static string query_log = "magic_book.dbo.query_log ";
        public static string not_exists_isbn = " magic_book.dbo.not_exists_isbn";

        public static string T_DIY_Book_Review_Backup = "magic_book.dbo.T_DIY_Book_Review_Backup";

        public static string UsedActivationCode="[magic_book].[dbo].[UsedActivationCode]";

        public static string T_Switch_Control="magic_book.dbo.T_Switch_Control";

        /// <summary>
        /// 梦想钻购买记录
        /// </summary>
        public static string T_MXZ_Order_Info = "[magic_book].[dbo].[T_MXZ_Order_Info]";

        /// <summary>
        /// 梦想钻变动记录
        /// </summary>
        public static string T_User_MXZ_History="magic_book.dbo.T_User_MXZ_History";

        /// <summary>
        /// 图书二维码下载资源表（统计工具功能）
        /// </summary>
        public static string T_CountToolResource="magic_book.dbo.T_CountToolResource";



        #region 4D课件库
        /// <summary>
        /// 科目表
        /// </summary>
        public static string T_SubjectInfo = "[mxr_cw].[dbo].[T_SubjectInfo]";
        /// <summary>
        /// 年级表
        /// </summary>
        public static string T_Grade = "[mxr_cw].[dbo].[T_Grade]";

        /// <summary>
        /// 年级与科目关联表
        /// </summary>
        public static string T_ClassInfo = "[mxr_cw].[dbo].T_ClassInfo";

        /// <summary>
        /// 出版社
        /// </summary>
        public static string T_EditionInfo = "[mxr_cw].[dbo].[T_EditionInfo]";

        /// <summary>
        /// 书表
        /// </summary>
        public static string Mxr_4DCW_Book = "[mxr_cw].[dbo].[mxr_4dcw_book]";

        /// <summary>
        /// 章
        /// </summary>
        public static string Mxr_4DCW_Book_Chapter = "[mxr_cw].[dbo].[mxr_4dcw_book_chapter]";

        /// <summary>
        /// 节
        /// </summary>
        public static string Mxr_4DCW_Book_Section = "[mxr_cw].[dbo].[mxr_4dcw_book_section]";
        /// <summary>
        /// 课件信息表（线上）
        /// </summary>
        public static string T_CWInfo = "[mxr_cw].[dbo].[T_CWInfo]";
        /// <summary>
        /// 节与课件的关联表
        /// </summary>
        public static string T_CWSection = "[mxr_cw].[dbo].[T_CWSection]";
        #endregion

        #region 消息库
        /// <summary>
        /// 消息表
        /// </summary>
        public static string MessageInfo = "MessageData.dbo.MessageInfo";

        public static string MessageContent = "MessageData.dbo.MessageContent";

        public static string MessageContent_Temp = "MessageData.dbo.MessageContent_Temp";

        /// <summary>
        /// 回复表
        /// </summary>
        public static string ReplyMessage = "[MessageData].[dbo].[ReplyMessage]";

        /// <summary>
        /// 临时消息表
        /// </summary>
        public static string MessageInfo_Temp = "MessageData.dbo.MessageInfo_Temp";

        public static string MessageToUser = "MessageData.[dbo].[MessageToUser]";

        /// <summary>
        /// 消息审核表
        /// </summary>
        public static string messageAudit = "MessageData.[dbo].[messageAudit]";

        /// <summary>
        /// 用户消息表
        /// </summary>
        public static string UserMessageList = "[MessageData].dbo.[UserMessageList]";

        /// <summary>
        /// 短消息回复表
        /// </summary>
        public static string MessageReplyContent = "MessageData.dbo.MessageReplyContent";

        public static string MessagePraiseList = "MessageData.dbo.MessagePraiseList";

        public static string RewardCoin = "[MessageData].dbo.[RewardCoin]";

        /// <summary>
        /// 私信接收人列表
        /// </summary>
        public static string T_Private_Message_User_List = "messagedata.dbo.T_Private_Message_User_List";

        /// <summary>
        /// 私信临时主表
        /// </summary>
        public static string T_Private_Message_Info_Temp = "messagedata.dbo.T_Private_Message_Info_Temp";

        /// <summary>
        /// 私信群推表（扩展私信临时主表）
        /// </summary>
        public static string T_Private_Message_GroupPush="MessageData.dbo.T_Private_Message_GroupPush";

        /// <summary>
        /// 私信日志
        /// </summary>
        public static string T_Private_Message_Info_Temp_Log = "[MessageData].[dbo].[T_Private_Message_Info_Temp_Log]";

        /// <summary>
        /// 私信群发的主表
        /// </summary>
        public static string T_Private_Message_Info = "messagedata.dbo.T_Private_Message_Info";

        #endregion

        #region ARSHOP
        /// <summary>
        /// 运管平台用户信息表
        /// </summary>
        public static string MAS_ADMIN_ACCOUNT = "ARSHOP.DBO.MAS_ADMIN_ACCOUNT";

        /// <summary>
        /// 激活码申请表
        /// </summary>
        public static string MAS_ACTIVATION_CODE_APPLY = "ARSHOP.DBO.MAS_ACTIVATION_CODE_APPLY";

        public static string MAS_ACTIVATION_CODE = "ARSHOP.DBO.MAS_ACTIVATION_CODE";

        public static string MAS_ACTIVATION_CODE_PRODUCER = "ARSHOP.DBO.MAS_ACTIVATION_CODE_PRODUCER";

        /// <summary>
        /// 激活码表
        /// </summary>
        /// <returns></returns>
        public static string MAS_ACTIVATION_CODE_TEMP = "arshop.dbo.MAS_ACTIVATION_CODE_TEMP";

        /// <summary>
        /// 未用（即可以申请）的激活码表
        /// </summary>
        public static string MAS_ACTIVATION_CODE_NoUse = "arshop.dbo.MAS_ACTIVATION_CODE_NoUse";

        public static string MAS_ARGAME_INFORMATION = "arshop.dbo.MAS_ARGAME_INFORMATION";

        public static string MAS_ARGAME_REVIEW = "arshop.dbo.MAS_ARGAME_REVIEW";

        public static string MAS_ARGAME_INFORMATION_APPROVAL = "arshop.dbo.MAS_ARGAME_INFORMATION_APPROVAL";

        public static string MAS_Authorization = "arshop.dbo.MAS_Authorization";

        public static string MAS_AUTHORIZATION_ITEM = "arshop.dbo.MAS_AUTHORIZATION_ITEM";

        public static string MAS_MEUN_ITEMS = "arshop.dbo.MAS_MEUN_ITEMS";

        public static string MAS_MXR_PARA="arshop.dbo.MAS_MXR_PARA";

        public static string MAS_MAGIC_BOOK_REVIEW = "arshop.dbo.MAS_MAGIC_BOOK_REVIEW";

        public static string MAS_MOBILE_PROJ_REVIEW = "arshop.dbo.MAS_MOBILE_PROJ_REVIEW";

        public static string MAS_ADMIN_ROLE = "arshop.dbo.MAS_ADMIN_ROLE";

        public static string MAS_APPROVAL_TYPE = "arshop.dbo.MAS_APPROVAL_TYPE";

        public static string UserFlow = "arshop.dbo.UserFlow";

        public static string FlowTable = "[arshop].[dbo].[FlowTable]";

        public static string iYiKeResourceInfoTemp = "[iyike_4].[dbo].iYiKeResourceInfoTemp";

        public static string MAS_ADMIN_OPERATION_RECORDER = "[arshop].[dbo].MAS_ADMIN_OPERATION_RECORDER";

        public static string MAS_ADMIN_OPERATION_RECORDER_MOBILE = "[arshop].[dbo].MAS_ADMIN_OPERATION_RECORDER_MOBILE";

        public static string MAS_CHANNEL_NAME="ARSHOP.DBO.MAS_CHANNEL_NAME";

        public static string MAS_COLOR_CODE = "ARSHOP.DBO.MAS_COLOR_CODE";

        public static string MAS_NAVIGATION_NAME = "arshop.dbo.MAS_NAVIGATION_NAME";

        public static string MAS_CLASSICS_ORDER_PAY_HISTORY = "ARSHOP.DBO.MAS_CLASSICS_ORDER_PAY_HISTORY";

        public static string MAS_PC_ORDER_PAY_HISTORY = "ARSHOP.DBO.MAS_PC_ORDER_PAY_HISTORY";

        public static string MAS_PLUGIN_REVIEW = "ARSHOP.DBO.MAS_PLUGIN_REVIEW";

        public static string MAS_LOG = "ARSHOP.DBO.MAS_LOG";

        public static string MAS_USER_INFORMATION = "ARSHOP.DBO.MAS_USER_INFORMATION";

        public static string MAS_ORDER_RENEW = "ARSHOP.DBO.MAS_ORDER_RENEW";

        public static string MAS_ORDER_PAY_HISTORY = "ARSHOP.DBO.MAS_ORDER_PAY_HISTORY";

        public static string historyURL = "[arshop].[dbo].[historyURL]";

        public static string RecordOperation = "[arshop].[dbo].[RecordOperation]";

        public static string T_Qrcode_Management = "[arshop].[dbo].[T_Qrcode_Management]";

        public static string MAS_MAIL_CONTENT = "[arshop].[dbo].[MAS_MAIL_CONTENT]";

        public static string ERP_BarcodeInfo = "arshop.dbo.ERP_BarcodeInfo";

        public static string ERP_StorgeInfo = "arshop.dbo.ERP_StorgeInfo ";

        /// <summary>
        /// 视图
        /// </summary>
        public static string get_user_detail_infomation_v = "arshop.dbo.get_user_detail_infomation_v";

        public static string all_project_view = "arshop.dbo.all_project_view";

        public static string TemplateVsersions="arshop.dbo.TemplateVsersions";

        public static string MAS_TEMPLATE_INFO="arshop.dbo.MAS_TEMPLATE_INFO";

        /// <summary>
        /// 系统配置表
        /// </summary>
        public static string MAS_SYSTEMCONFIG = "[arshop].[dbo].[MAS_SYSTEMCONFIG]";
        #endregion

        #region UserAccount
        public static string UserAccount = "UserData.dbo.UserAccount";
        #endregion

        #region CTF
        public static string QR_Code = "[CTF].[dbo].[QR_Code]";
        #endregion

        #region 爱医课
        public static string mxr_examination_book = "[ayk].[dbo].[mxr_examination_book]";

        public static string userInfo_ayk = "[ayk].[dbo].userInfo";

        public static string messageReply_ayk = "[ayk].[dbo].messageReply";

        public static string messageReply_CS = "[ayk].[dbo].messageReply_CS";

        public static string PaymentLog_ayk = "[ayk].[dbo].PaymentLog";
        public static string feed_back_ayk = "[ayk].[dbo].feed_back";
        public static string T_InvitationCodeUsedLog_ayk = "[ayk].[dbo].T_InvitationCodeUsedLog";

        public static string book_store_data_ayk = "[ayk].[dbo].book_store_data";

        public static string UserMessage_ayk = "[ayk].[dbo].UserMessage";

        public static string messageInfo_ayk = "[ayk].[dbo].messageInfo";

        public static string messageContent_ayk = "[ayk].[dbo].messageInfo";

        public static string MAS_ADMIN_ACCOUNT_ayk = "[ayk].[dbo].MAS_ADMIN_ACCOUNT";

        public static string reply_feed_back_ayk = "[ayk].[dbo].reply_feed_back";


        public static string get_user_detail_infomation_v_ayk = "[ayk].[dbo].dbo.get_user_detail_infomation_v";
        #endregion

        #region 数据收集库book_store_data_collection
        public static string T_QR_Statistics = "book_store_data_collection.dbo.T_QR_Statistics";

        public static string T_QR_Statistics_Detail = "book_store_data_collection.dbo.T_QR_Statistics_Detail";

        /// <summary>
        /// 图书二维码打开、扫描、下载记录
        /// </summary>
        public static string T_CountToolHistory = "book_store_data_collection.DBO.T_CountToolHistory";

        /// <summary>
        /// 图书二维码打开、扫描、下载记录日统计表
        /// </summary>
        public static string T_CountToolCount = "[book_store_data_collection].[dbo].[T_CountToolCount]";

        /// <summary>
        /// 二维码下载记录表
        /// </summary>
        public static string T_DownLoaded_Book_Data=" [book_store_data_collection].[dbo].[t_downloaded_book_data]";

        /// <summary>
        /// 二维码下载记录日统计表
        /// </summary>
        public static string T_Download_Book_Count="[book_store_data_collection].[dbo].[T_Download_Book_Count]";



        #endregion

        #region
        public static string T_DIYBooks = "golden_key_db.dbo.T_DIYBooks";
        #endregion

        #region mobile_dream_store
        public static string mxr_publiction="mobile_dream_store.dbo.mxr_publiction";

        public static string mxr_publiction_review = "mobile_dream_store.dbo.mxr_publiction_review";

        public static string mxr_model_view_demo_info="mobile_dream_store.dbo.mxr_model_view_demo_info";

        #endregion

        #region 美慧树
        public static string SyllabusInfo = "MeiHuishu.dbo.SyllabusInfo";

        public static string ThemeInfo = "MeiHuishu.dbo.ThemeInfo";

        public static string SyllabusContent = "MeiHuishu.dbo.SyllabusContent";

        public static string BookInfo = "MeiHuishu.dbo.BookInfo";
        #endregion
    }
}