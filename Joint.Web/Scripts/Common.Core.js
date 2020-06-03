

window.GetJsonData = "";
window.ReturnedResult = "";
function CallAjax(param, successFunc, failFunc) {
	GetJsonData = "";
	param.type = param.type || "POST";
	param.isAsync = param.isAsync == undefined ? true : param.isAsync;
	param.isCache = param.isCache || false;
	param.dataType = param.dataType || "json";
	param.data = param.data || "";
	$.ajax({
		async: param.isAsync,
		type: param.type,
		url: param.reqUrl,
		data: param.data,
		dataType: param.dataType,
		cache: param.isCache,
		success: function (result) {
		    ReturnedResult = result;

			if (result.success) {
				if (result.GetJsonData != undefined) { GetJsonData = result.GetJsonData; }
				if (typeof successFunc == "function") { successFunc(); } else { layer.msg(result.msg); }
			}
			else {
				if (typeof failFunc == "function") { failFunc(); } else { layer.msg(result.msg); }
			}
		},
		error: function (msg) {
			alert("网络连接失败！");
		}
	});
}
function ReloadPage() {
	location.reload();
}
//返回上级页面
function GoBack(defaultUrl) {
	if (window.history.length > 1) {history.go(-1);}
	else {window.location.href = defaultUrl;}
}
//重置表单信息
function ResetForm() {
	$(".SearchBar input,.SearchBar select").val("");
}
function ResetForm2() {
    $(".SearchBar input").val("");
    $(".SearchBar select").find("option:eq(0)").attr("selected", true);
}
//获取浏览器参数的值
function GetUrlPV(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}



