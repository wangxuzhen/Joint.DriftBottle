function setTab(name, cursel, n) {
    //
    for (i = 1; i <= n; i++) {
        var currTab = $("#" + name + i);
        var currCon = $("#" + "con_" + name + "_" + i);
        if (i == cursel) {
            currTab.addClass("hover");
            currCon.show();
        }
        else {
            currTab.removeClass("hover");
            currCon.hide();
        }
       
        //var menu = document.getElementById(name + i);
        //var con = document.getElementById("con_" + name + "_" + i);
        //menu.className = i == cursel ? "hover" : "";
        //con.style.display = i == cursel ? "block" : "none";
    }
}