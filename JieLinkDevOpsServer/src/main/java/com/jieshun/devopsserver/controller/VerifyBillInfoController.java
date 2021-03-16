package com.jieshun.devopsserver.controller;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.jieshun.devopsserver.bean.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.multipart.MultipartHttpServletRequest;
import org.springframework.web.bind.annotation.*;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLEncoder;
import java.text.SimpleDateFormat;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Iterator;
import java.util.List;

import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.openxml4j.opc.OPCPackage;
import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.CellType;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.xssf.eventusermodel.XSSFReader;
import org.apache.poi.xssf.model.SharedStringsTable;
import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

import com.jieshun.devopsserver.service.VerifyBillInfoService;

/**
 * 对账管理Controller
 * 
 * @author ggd
 *
 */
@RestController
@RequestMapping(value = "/VerifyBill")
public class VerifyBillInfoController {
	
	//文件存储路径
	public static final String FilePath = "C://VerifyBillFile//";
	
	//状态码
	public enum StatusCode
	{
		//需要提供远程 0
		NeedSuportRemote(0),
		//等待研发处理 1
		WaitForDeveloper (1),
		//语句已生成，自助处理 2
		DoItYourSelf(2) ,
		//研发已处理 3
		DeveloperDone(3),
		//已结单，完成
		Finsih(4);
		private final int value;
		StatusCode(int value) {
	        this.value = value;
	    }
	    public int getValue() {
	        return value;
	    }
	}
	private static final Logger log = LoggerFactory.getLogger(VerifyBillInfoController.class);

	
	@Autowired
	VerifyBillInfoService VerifyBillInfoService;
	/**
	 * 根据分页查询对账信息
	 * 
	 * @param orderNo 工单号
	 * @param start   起始索引
	 * @param end     结束索引
	 * @return 分页对象
	 */
	@RequestMapping(value = "/getVerifyBillInfoWithPages", method = RequestMethod.GET)
	public PageSet<VerifyBillInfo> getVerifyBillInfoWithPages(String orderNo, int start, int end) {
		return VerifyBillInfoService.getVerifyBillInfoWithPages(orderNo, start, end);
	}
	
	/**
	 * 根据信息查找向日葵
	 * @param projectshoperno 商户号
	 * @param projectno 项目编号 就是车场编号
	 * @param projectname 项目名
	 * @return
	 */
	@RequestMapping(value = "/SearchSunloginByInfo", method = RequestMethod.GET)
	public ReturnData SearchSunloginByInfo(String project_shoper_no,String project_no,String project_name,String versiontype){
		String result = VerifyBillInfoService.SearchSunloginByInfo(project_shoper_no,project_no,project_name,versiontype);
		if(result == "")
		{
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
		else
		{
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), result);
		}
	}
	
	
	/**
	 * 增加紧急度
	 * @param id ID
	 */
	@RequestMapping(value = "/addEmergencyById", method = RequestMethod.PUT)
	public int addEmergencyById(int id){
		return VerifyBillInfoService.addEmergencyById(id);
	}
	
	/**
	 * 删除对账信息
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/deleteVerifyBillInfoById", method = RequestMethod.DELETE)
	public ReturnData deleteVersionInfoById(int id) {
		int result = VerifyBillInfoService.deleteVerifyBillInfoById(id);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	/**
	 * 验收对账
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/finishVerifyBillInfoById", method = RequestMethod.PUT)
	public ReturnData finishVerifyBillInfoById(int id) {
		int result = VerifyBillInfoService.finishVerifyBillInfoById(id);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	/**
	 * 发布对账信息
	 * 
	 * @param workOrderNo
	 * @param standVersion
	 * @param versionType
	 * @param compileDate
	 * @param versionDescribe
	 * @param downloadMsg
	 * @return
	 */
	@RequestMapping(value = "/addVerifyBillInfo", method = RequestMethod.POST)
	public ReturnData addVerifyBillInfo(String publisher_name,String project_shoper_no,String project_no,String project_name,
			String versionType,String version,int nonstandard,String project_remote,String project_remote_password,String describe,String project_task,
			String uploadfilename)
	{
		try
		{
			VerifyBillInfo verifybillInfo = new VerifyBillInfo();
			
			verifybillInfo.setPublisherName(publisher_name);
			verifybillInfo.setProjectShoperNo(project_shoper_no);
			verifybillInfo.setProjectNo(project_no);
			verifybillInfo.setProjectName(project_name);
			verifybillInfo.setProjectBigVersion(versionType);
			verifybillInfo.setProjectVersion(version);
			verifybillInfo.setProjectIsNonstandard(nonstandard);
			verifybillInfo.setProjectRemote(project_remote);
			verifybillInfo.setProjectRemotePassword(project_remote_password);
			verifybillInfo.setProjectRemark(describe);
			verifybillInfo.setUploadfilename(uploadfilename);
			
			String taskstring = "";
			if(project_task.contains("补录"))
				taskstring += "补录 ";
			if (project_task.contains("补推"))
				taskstring += "补推 ";
			if (project_task.contains("删除"))
				taskstring += "删除 ";
			if (project_task.contains("退款"))
				taskstring += "退款 ";
			if (project_task.contains("查原因"))
				taskstring += "查因 ";
			verifybillInfo.setProjectTask(taskstring);
			
			//紧急度默认为0 是否删除默认为0
			verifybillInfo.setEmergency(0);
			verifybillInfo.setIsdelete(0);

			//无远程的话状态为需要远程
			if(project_remote.equals("无") )
				verifybillInfo.setStatus(StatusCode.NeedSuportRemote.value);
			else
				verifybillInfo.setStatus(StatusCode.WaitForDeveloper.value);
			
			//如果不需要查原因，以及不需要补推，以及没有退款问题，并且是jielink的 就不需要研发处理了
			if(!taskstring.contains("查因") && !taskstring.contains("补推") && !taskstring.contains("退款") && versionType.equals("0"))
			{
				verifybillInfo.setStatus(StatusCode.DoItYourSelf.value);
			}
			
			//自动生成SQL语句
			verifybillInfo.setAutosql(MakeSql(versionType,uploadfilename,taskstring));
			
			//保存信息
			int result = VerifyBillInfoService.addVerifyBillInfo(verifybillInfo);
			
			if (result > 0) {
				return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
			} else {
				return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
			}
		}
		catch(Exception	 e)
		{
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), e.toString());
		}
	}
	
	/**
	 * 生成对账SQL语句
	 * @param VersionType
	 * @param FileName
	 * @param taskstring
	 * @return
	 */
	private String MakeSql(String VersionType,String FileName,String taskstring)
	{
		String ret = "";
		if(!VersionType.equals("0"))
			return "仅支持jielink的自动生成sql语句";
		
		if(FileName == "")
			return "没有检测到上传的文件名，无法生成SQL语句";
		
		File file = new File(FilePath + FileName);
		if(!file.exists())
			return "没有检测到文件，如果文件较大可能还在上传中，等待1-2分钟";

		if(!taskstring.contains("删除") && !taskstring.contains("补录"))
		{
			return "不包含删除或者补录任务，不能自动生成脚本";
		}
		
		boolean xlsx = true;
		//XSSFWorkbook xssfWorkbook = null;
		//XLSX
		XSSFSheet sheetAt = null;
		//XLS
		HSSFSheet HsheetAt = null;
		int totalRowNum  = 0,totalCellNum  = 0;
		try {

			FileInputStream inputStream = new FileInputStream(FilePath + FileName);
			if(FileName.contains(".xlsx"))
			{
				//这里创建xssfworkbook非常慢 不知道为什么 1M以上文件基本上内存要溢出了
				//这样会快一点 https://blog.csdn.net/ylforever/article/details/80955595				
				OPCPackage opcPackage = OPCPackage.open(file);
				XSSFWorkbook xssfWorkbook = new XSSFWorkbook(opcPackage);

				sheetAt = xssfWorkbook.getSheetAt(0);
				xlsx = true;
				xssfWorkbook.close();
			}
			else if(FileName.contains(".xls"))
			{
				//HSSF还不支持opcPackage导入...
				HSSFWorkbook hssfWorkbook = new HSSFWorkbook(inputStream);
				HsheetAt = hssfWorkbook.getSheetAt(0);
				xlsx = false;
				hssfWorkbook.close();
			}
			else 
				return "文件未检测到xlsx或者xls后缀";
		} catch (Exception e) {
			// TODO 自动生成的 catch 块
			//e.printStackTrace();
			return e.toString();
		}

		try {
			if (sheetAt == null && HsheetAt == null)
				return "获取工作簿失败";
			// 如果任务有删除或者补录，那么生成SQL语句 注意不可以同时生成删除和补录
			// 获得表头
			Row rowHead;
			if (xlsx)
				rowHead = sheetAt.getRow(0);
			else
				rowHead = HsheetAt.getRow(0);
			// 获得总行数 总列
			if (xlsx)
				totalRowNum = sheetAt.getLastRowNum();
			else
				totalRowNum = HsheetAt.getLastRowNum();
			if (totalRowNum > 51)
				return "第一个工作簿中数据超过50条，请确认是否放对地方了或者有多余数据？";
			totalCellNum = rowHead.getLastCellNum();
			
			// 把所有标题导到MAP
			Map<String, Integer> titles = new HashMap<String, Integer>();
			for (int k = 0; k < totalCellNum; k++) {
				Cell cell = rowHead.getCell(k);
				titles.put(cell.toString(), k);
			}

			// 对表头进行校验，如果缺少某些表头，必定无法正常生成，直接返回错误
			if(!titles.containsKey("订单号"))
				ret += "未找到 订单号 表头，请检查excel数据";
			//高版本为服务开始时间 服务结束时间，部分低版本为入场时间，计费时间
			if(!titles.containsKey("服务开始时间") && !titles.containsKey("入场时间"))
				ret += "未找到 服务开始时间 或者 入场时间 表头，请检查excel数据";
			if(!titles.containsKey("服务结束时间") && !titles.containsKey("计费时间"))
				ret += "未找到 服务结束时间 或者 计费时间 表头，请检查excel数据";
			if(!titles.containsKey("收费金额") && !titles.containsKey("计费金额"))
				ret += "未找到 收费金额 或者 计费金额 表头，请检查excel数据";
			if(!titles.containsKey("支付时间"))
				ret += "未找到 支付时间 表头，请检查excel数据";
			if(!titles.containsKey("支付方式"))
				ret += "未找到 支付方式 表头，请检查excel数据";
			if(!titles.containsKey("应收金额"))
				ret += "未找到 应收金额 表头，请检查excel数据";			
			if(!titles.containsKey("车牌"))
				ret += "未找到 车牌 表头，请检查excel数据";
			if(!titles.containsKey("优惠金额"))
				ret += "未找到 优惠金额 表头，请检查excel数据";
			if(ret != "")
				return ret;
			
			// 补录语句生成
			for (int i = 1; i <= totalRowNum; i++) {
				Row row;
				if (xlsx)
					row = sheetAt.getRow(i);
				else
					row = HsheetAt.getRow(i);
				
				// 订单号所在的列
				String orderID = row.getCell(titles.get("订单号")).getStringCellValue().toString();
				ret += "-- " + orderID + "\r\n";
				
				//开始时间结束时间
				String inTime = "";
				if(titles.containsKey("服务开始时间"))
					inTime = row.getCell(titles.get("服务开始时间")).getStringCellValue().toString();
				else
					inTime = row.getCell(titles.get("入场时间")).getStringCellValue().toString();
				
				String feesTime = "";
				if(titles.containsKey("服务结束时间"))
					row.getCell(titles.get("服务结束时间")).getStringCellValue().toString();
				else
					row.getCell(titles.get("计费时间")).getStringCellValue().toString();
				
				String fees = "";
				if(titles.containsKey("收费金额"))
				{
					if (row.getCell(titles.get("收费金额")).getCellType() == CellType.NUMERIC)
						fees = String.valueOf(row.getCell(titles.get("收费金额")).getNumericCellValue()).replace("元", "");
					else if (row.getCell(titles.get("收费金额")).getCellType() == CellType.STRING)
						fees = row.getCell(titles.get("收费金额")).getStringCellValue().toString().replace("元", "");
				}
				else
				{
					if (row.getCell(titles.get("计费金额")).getCellType() == CellType.NUMERIC)
						fees = String.valueOf(row.getCell(titles.get("计费金额")).getNumericCellValue()).replace("元", "");
					else if (row.getCell(titles.get("计费金额")).getCellType() == CellType.STRING)
						fees = row.getCell(titles.get("计费金额")).getStringCellValue().toString().replace("元", "");
				}
					
				String accountReceivable = fees;
				String actualPaid = fees;
				String payTime = row.getCell(titles.get("支付时间")).getStringCellValue().toString();
				String paytype = row.getCell(titles.get("支付方式")).getStringCellValue().toString();
				int payTypeID = paytype.equals("微信") ? 2 : paytype.equals("支付宝") ? 1 : paytype.equals("捷顺金科") ? 22 : 0;
				Date day = new Date();
				SimpleDateFormat df = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
				String createTime = df.format(day);
				//String money = row.getCell(titles.get("应收金额")).getStringCellValue().toString().replace("元", "");
				String credentialNO = row.getCell(titles.get("车牌")).getStringCellValue().toString().replace("-", "")
						.trim();
				String plate = row.getCell(titles.get("车牌")).getStringCellValue().toString().replace("-", "").trim();
				String payTypeName = row.getCell(titles.get("支付方式")).getStringCellValue().toString();
				
				String benefit = "";
				if (row.getCell(titles.get("优惠金额")).getCellType() == CellType.NUMERIC)
					benefit = String.valueOf(row.getCell(titles.get("优惠金额")).getNumericCellValue()).replace("元", "");
				else if (row.getCell(titles.get("优惠金额")).getCellType() == CellType.STRING)
					benefit = row.getCell(titles.get("优惠金额")).getStringCellValue().toString().replace("元", "");
				
				int paid = 0;
				int derate = 0;
				int exchange = 0;
				int smallChange = 0;

				//重要字段校验
				if(plate.isBlank()) {
					ret += "车牌为空,该条无法生成";
					continue;
					}
				if(inTime.isBlank()){
					ret += "入场时间为空,该条无法生成";
					continue;
				}
				
				if (taskstring.contains("补录")) {
					ret += "-- 生成了补录语句" + "\r\n";
					String str = String.format(
							"select EnterRecordID into @enterRecordId from box_enter_record where plate='%s' and EnterTime='%s';",
							plate, inTime);
					String str2 = String.format(
							"INSERT INTO `box_bill` (`BGUID`,`OrderId`,`InTime`,`FeesTime`,`Fees`,`Benefit`,`Derate`,`AccountReceivable`,`Paid`,`ActualPaid`,`Exchange`,`SmallChange`,`Cashier`,`PayTime`,`discountPicturePath`,`PayTypeID`,`ChargeType`,`ChargeDeviceID`,`OperatorID`,`OperatorName`,`CloudID`,`EnterRecordID`,`CreateTime`,`OrderType`,`Money`,`Status`,`SealTypeId`,`SealTypeName`,`Remark`,`ReplaceDeduct`,`AppUserId`,`TrusteeFlag`,`EventType`,`PayFrom`,`DeviceID`,`CredentialNO`,`CredentialType`,`CashierName`,`PersonNo`,`PersonName`,`discounts`,`ChargeDeviceName`,`FreeMoney`,`UpLoadFlag`,`Plate`,`OnlineExchange`,`PayTypeName`,`ExtStr1`,`ExtStr2`,`ExtStr3`,`ExtStr4`,`ExtStr5`,`ExtInt1`,`ExtInt2`,`ExtInt3`,`CashTotal`,`ParkNo`) VALUES (uuid(), '%s', '%s', '%s', '%s', '%s', '%s', '%s', '%d', '%s', '%d', '%d', '9999', '%s', '', '%d', '0', '', '9999', '超级管理员', '%s', @enterRecordId, '%s', '1', '%s', '1', '54', '临时用户A', '人工补录', '0', '', '0', '1', 'jieshun', null, '%s', '163', '超级管理员', '', '', '', null, '0.00', '1', '%s', '0.00', '%s',null, null, null, null, null, '0', '0', '0','0.00', '00000000-0000-0000-0000-000000000000');",
							orderID, inTime, feesTime, fees, benefit, derate, accountReceivable, paid,
							accountReceivable, exchange, smallChange, payTime, payTypeID, orderID, createTime, fees,
							credentialNO, plate, payTypeName);
					ret += str;
					ret += str2 + "\r\n";
				} else if (taskstring.contains("删除")) {
					ret += "-- 生成了删除语句" + "\r\n";
					ret += String.format("delete from 'box_bill' where orderid = '%s';", orderID);
					ret += "\r\n";
				}
			}
			ret += "-- 语句结束 --";

			return ret;
		}
		catch(Exception e)
		{
			return ret + "\r\n" + "-- " + e.toString();
		}
	}
	
	
	/**
	 * 研发确认，在备注后加remark
	 * @param id
	 * @param remark 
	 * @param user 确认时的登录名
	 * @return
	 */
	@RequestMapping(value = "/ConfirmVerifyBillById", method = RequestMethod.PUT)
	public ReturnData ConfirmVerifyBillInfo(int id,String remark,String user)
	{
		int result = VerifyBillInfoService.confirmVerifyBillInfoById(id,remark,user);
		if (result > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	/**
	 * 获取SQL语句
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/GetAutoSqlById", method = RequestMethod.GET)
	public ReturnData GetAutoSql(int id)
	{
		String result = VerifyBillInfoService.GetAutoSqlById(id);
		if (result != "") {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), result);
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	/**
	 * 下载文件 返回文件流
	 * @param req
	 * @param resp
	 * @return
	 */
	@RequestMapping(value = "/GetFile/{time}", method = RequestMethod.POST)
	//public Blob GetFile(Integer id)
	public void GetFile(Integer id, HttpServletResponse response)
	{
		try
		{
			String result = VerifyBillInfoService.GetUrlById(id);

			File file = new File(FilePath + result);
			if(!file.exists())
			{
				return ;
			}
			
			//FileOutputStream outputStream = new FileOutputStream(FilePath + result);
			if(result != "")
			{		
				response.setCharacterEncoding("UTF-8");
				OutputStream out = null;
				//System.out.println("==========getFileOutputStream=========文件路径:"+path);
		        //File file = new File(FilePath + result);    //1、建立连接
		        BufferedInputStream is = null;
		        byte[] b = new  byte[4096];
		        int len = 1024;
		        // 清空response
		        response.reset();
		        response.setContentType("application/x-download");//设置response内容的类型 普通下载类型
		        response.setHeader("filename", URLEncoder.encode(result, "UTF-8"));//设置头部信息
				//response.setHeader("Content-disposition","attachment;filename="+ URLEncoder.encode(file.getName(), "UTF-8"));//设置头部信息
				out = response.getOutputStream();
				is = new BufferedInputStream(new FileInputStream(file));
				while((len= is.read(b)) != -1) {
					out.write(b,0,len);
				}
				
				out.flush();
				if(is!=null)
					is.close();
				if(out!=null)
					out.close();	
			}
			else
			{
				return ;
			}
		}
		catch(Exception e)
		{
			return ;
		}
//		if (result != "") {
//			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), inputStream);
//		} else {
//			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
//		}
	}
	
	/**
	 * 根据ID补充远程
	 * @param id
	 * @param remote
	 * @param remote_password
	 * @return
	 */
	@RequestMapping(value = "/SetRemoteById", method = RequestMethod.PUT)
	public ReturnData SetRemoteById(int id,String remote,String remote_password)
	{
		int ret = VerifyBillInfoService.SetRemoteById(id,remote,remote_password);
		if(ret > 0) {
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		} else {
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), ReturnStateEnum.FAILD.getMessage());
		}
	}
	
	/**
	 * 上传文件
	 * @param req
	 * @param resp
	 * @return
	 */
	@RequestMapping(value="/upload/{time}",method={RequestMethod.POST})
	//@RequestMapping(value="/upload",method={RequestMethod.POST})
	@ResponseBody
	public ReturnData upload(HttpServletRequest req, HttpServletResponse resp){
		try {
			//获取上传上来的文件并且保存到路径下
			MultipartHttpServletRequest multipartMap = (MultipartHttpServletRequest)req;
			Map<String, MultipartFile> fileMap = multipartMap.getFileMap();
			String str = saveFile(fileMap.get("file"));
			if(str.isEmpty())
				return new ReturnData(ReturnStateEnum.FAILD.getCode(), "文件保存失败！请联系开发人员！");
			
			String orgfilename = fileMap.get("file").getOriginalFilename();
			//由于文件上传较慢，可能导致表单提交时SQL语句未生成，上传文件后再检查一次
			String ret = VerifyBillInfoService.CheckSQL(orgfilename);
			if(ret != "")
			{
				//重新生成一下 VersioType这里，既然已经检测文件是否存在了，那肯定通过jielink验证
				int result = VerifyBillInfoService.SetSqlByFileName(orgfilename,MakeSql("0",orgfilename,ret));
				
				//MakeSql("0",fileMap.get("file").getOriginalFilename(),ret);
			}
			
			return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
		}
		catch(Exception e)
		{
			return new ReturnData(ReturnStateEnum.FAILD.getCode(), e.toString());
		}
	}
	
	/**
	 * 测试用
	 * @param req
	 * @param resp
	 * @return
	 */
	@RequestMapping(value="/uploadTest/{time}",method={RequestMethod.POST})
	//@RequestMapping(value="/upload",method={RequestMethod.POST})
	public ReturnData uploadtest(HttpServletRequest req, HttpServletResponse resp){
		MultipartHttpServletRequest multipartMap = (MultipartHttpServletRequest)req;
		Map<String, MultipartFile> fileMap = multipartMap.getFileMap();

		String str = saveFile(fileMap.get(0));
		return new ReturnData(ReturnStateEnum.SUCCESS.getCode(), ReturnStateEnum.SUCCESS.getMessage());
	}
	
	
	/***
	 * 保存文件到FilePath的目录，文件名取file
	 * @param 
	 * @param file
	 * @return
	 */
	private String saveFile(MultipartFile file) {
        try {
            if(file != null && !file.isEmpty()) {
                 
                String filePath = FilePath + file.getOriginalFilename();
                File fp = new File(new File(filePath).getParent());
                if(!fp.exists()){
                    fp.mkdirs();
                }
                DataOutputStream out = new DataOutputStream(new FileOutputStream(filePath));
                InputStream is = null;
                try {
                    is = file.getInputStream();
                    byte[] b=new byte[is.available()];
                    is.read(b);
                    out.write(b);
                    return filePath ;
                } catch (Exception e) {
                    throw new RuntimeException(e);
                } finally {
                    if (is != null) {
                        is.close();
                    }
                    if (out != null) {
                        out.close();
                    }
                }
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        return "";
    }
//
//	/**
//	 * 根据分页查询版本下载情况
//	 *
//	 * @param standVersion 工单号
//	 * @param startIndex   起始索引
//	 * @param endIndex     结束索引
//	 * @return 分页对象
//	 */
//	@RequestMapping(value = "/getVersionDownloadInfoWithPages", method = RequestMethod.GET)
//	public PageSet<VerifyBillInfo> getVersionDownloadInfoWithPages(String standVersion, int startIndex, int endIndex) {
//		return VerifyBillInfoService.getVersionDownloadInfoWithPages(standVersion, startIndex, endIndex);
//	}
//
//	/**
//	 * 根据分页查询版本下载情况
//	 *
//	 * @param versionInfoKeyId VersionInfo的主键ID
//	 * @param startIndex   起始索引
//	 * @param endIndex     结束索引
//	 * @param enableAll	   是否查询出所有数据 0所有（不看start、end），1分页
//	 * @return 分页对象
//	 */
//	@RequestMapping(value = "/getApplyInfoByWorkOrderNoWithPages", method = RequestMethod.GET)
//	public PageSet<ApplyInfo> getApplyInfoByWorkOrderNoWithPages(int versionInfoKeyId, int startIndex, int endIndex, int enableAll) {
//		return VerifyBillInfoService.getApplyInfoByWorkOrderNoWithPages(versionInfoKeyId, startIndex, endIndex, enableAll);
//	}
}
