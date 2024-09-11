var booking = {
    location: '',
    area: '',
    province: '',
    provinceName: '',
    districtName: '',
    cont: '',
    code: '',
    cd: '',
};

var color = {
    area_cd: '',
    province_cd: '',
    district_cd: '',
    cont_cd: '',
};

var trackColor = {
    area_cd: '',
    province_cd: '',
    district_cd: '',
};

let chooseColor = "#2461A2";
let baseColor = "#E9F1FC";
var autoArea = 0;
var autoProvince = 0;
var begin = true;

window.onload = async function () {
     await Loading();
};

async function sentUpdateOrCreate(para) {
    var response;
    var URL = '/Home/' + para;
    if (para == "Create") {
        response = await $.ajax({
            type: 'POST',
            url: URL,
            data: {
                cd: booking.cd,
                cont: booking.cont,
                location: booking.provinceName + " - " + booking.districtName,
            },
            dataType: 'JSON',
            async: true,
            error: function (request, status, error) {
                console.log("temp1");
            },
            success: async function (data) {
            },
        });
        return response
    }
    else {
        response = await $.ajax({
            type: 'POST',
            url: URL,
            data: {
                cd: booking.cd,
                code: booking.code,
                cont: booking.cont,
                location: booking.provinceName + " - " + booking.districtName,
            },
            dataType: 'JSON',
            async: true,
            error: function (request, status, error) {
                console.log("temp2");
            },
            success: async function (data) {
            },
        });
        return response
    }
   
}

function UpdateOrCreate() {

    var message = "temp";
    //if (booking.code != "") {
    //    try {
    //        var res = await sentUpdateOrCreate("Update");
    //        console.log("temp", res);
    //    } catch (e) {
    //        console.log(e);
    //    }
    //}
    //else {
    //    try {
    //        var res = await sentUpdateOrCreate("Create");
    //        console.log("temp", res);
    //    } catch (e) {
    //        console.log(e);
    //    }
    //}
    showMessageBox(message, message, 'text', 'small', 10000);
}

async function Loading() {
    let code = document.getElementById('code').textContent;
    
    if (code != '') {
        document.getElementById("code").innerHTML = "Mã Booking: " + code;

        booking.code = document.getElementById('code').textContent;
        booking.area = document.querySelector(".BOOKING_DC_AREA_CD").textContent;
        booking.provinceName = document.querySelector(".BOOKING_DC_PROVINCE_NAME").textContent;
        booking.province = document.querySelector(".BOOKING_DC_PROVINCE_CD").textContent;
        booking.districtName = document.querySelector(".BOOKING_DC_DISTRICT_NAME").textContent;
        booking.cont = document.querySelector(".BOOKING_DC_TYPE_CONT").textContent;
        booking.cd = document.querySelector(".BOOKING_DC_DISTRICT_CD").textContent;


        await Click_Area(booking.area);
        await ChooseItem('area-item', document.querySelector('[cd="' + booking.area + '"]'));
        await Click_Province(booking.province);
        await ChooseItem('province-item', document.querySelector('[cd="' + booking.province + '"]'));
        Click_District(booking.cd);
        await ChooseItem('district-item', document.querySelector('[cd="' + booking.cd + '"]'));
        Click_Cont(booking.cont);
        await ChooseItem('box_cont', document.querySelector('[cd="' + booking.cont + '"]'));
        begin = false;
    }
    else {
        document.getElementById("changeHeader").innerHTML = "Tạo mới Booking";

        Click_Cont('cont_20');
        await ChooseItem('box_cont', document.querySelector('[cd="' + booking.cont + '"]'));

        autoArea = 1;
        autoProvince = 1;
        await defaultChoice("");
        begin = false;
    }
};

async function defaultChoice(order) {
    var distristID;
    var areaID = await getFirst("", "", order);
    var provinceID;

    if (autoArea) {
        if (!begin) {
            provinceID = await getFirst(1, booking.area);
        }
        else {
            provinceID = await Click_Area(areaID.getAttribute('cd'));
            await ChooseItem('area-item', document.querySelector('[cd="' + booking.area + '"]'));
        }
    }
    if (autoProvince) {
        if (!begin) {
            if (order == "1") {
                distristID = await Click_Province(provinceID.getAttribute('cd'));
                await ChooseItem('province-item', document.querySelector('[cd="' + booking.province + '"]'));
            }
            else {
                distristID = await getFirst(2, booking.province);
            }
        }   
        else {
            distristID = await Click_Province(provinceID.getAttribute('cd'));
            await ChooseItem('province-item', document.querySelector('[cd="' + booking.province + '"]'));
        }
    }

    Click_District(distristID.getAttribute('cd'));
    await ChooseItem('district-item', document.querySelector('[cd="' + booking.cd + '"]'));
}
 
async function getFirst(section, cd, clickSection) {
    var response = await $.ajax({
        type: 'POST',
        url: '/Home/clickFirst',
        data: {
            order: section,
            id: cd
        },
        dataType: 'JSON',
        async: true,
    });
    if (clickSection == "") {
        autoProvince = true;
        autoArea = true;
    }
    if (clickSection == "1") {
        autoProvince = true;
        autoArea = true;
    }
    if (clickSection == "2") {
        autoProvince = true;
        autoArea = false;
    }
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = response;
    return tempDiv.firstChild;
}

async function ChooseItem(classItem, control) {
    var button;
    if (classItem == 'area-item') {
        if (color.area_cd != '') {
            button = document.querySelector(`.${classItem}[cd="${color.area_cd}"]`);
            button.style.backgroundColor = baseColor;
            button.style.color = chooseColor;
        }
        color.area_cd = control.getAttribute('cd');
        if (trackColor.area_cd != color.area_cd && trackColor.district_cd != null) {
            $("#districtList").html("");
        }
        trackColor.area_cd = control.getAttribute('cd');
        color.province_cd = '';
        trackColor.province_cd = '';
        control.style.backgroundColor = chooseColor;
        control.style.color = baseColor;
        if (!begin) {
            await defaultChoice("1");
        }
    }
    else if (classItem == 'province-item') {
        if (color.province_cd != '' && trackColor.area_cd == color.area_cd) {
            button = document.querySelector(`.${classItem}[cd="${color.province_cd}"]`);
            button.style.backgroundColor = baseColor;
            button.style.color = chooseColor;
        }
        booking.provinceName = control.textContent;
        color.province_cd = control.getAttribute('cd');
        color.district_cd = '';
        trackColor.district_cd = '';
        control.style.backgroundColor = chooseColor;
        control.style.color = baseColor;

        if (!begin) {
            await defaultChoice("2");
        }
    }
    else if (classItem == 'district-item') {
        if (color.district_cd != '' && trackColor.area_cd == color.area_cd && trackColor.province_cd == color.province_cd) {
            button = document.querySelector(`.${classItem}[cd="${color.district_cd}"]`);
            button.style.backgroundColor = baseColor;
            button.style.color = chooseColor;
        }
        booking.districtName = control.textContent;
        color.district_cd = control.getAttribute('cd');
        trackColor.district_cd = control.getAttribute('cd');
        trackColor.province_cd = color.province_cd;
        control.style.backgroundColor = chooseColor;
        control.style.color = baseColor;
    }
    else if (classItem == 'box_cont') {
        if (color.cont_cd != '') {
            button = document.querySelector(`.${classItem}[cd="${color.cont_cd}"]`);
            button.style.backgroundColor = baseColor;
            button.style.color = chooseColor;
        }
        color.cont_cd = control.getAttribute('cd');
        control.style.backgroundColor = chooseColor;
        control.style.color = baseColor;
    }
}

function handleCloseDetail() {
    document.getElementsByClassName("overlay_menu")[0].classList.add("display_none_loading");
    document.getElementsByClassName("container_noti")[0].classList.add("display_none_loading");
}
function showMessageBox(text_title, text_cotent, type, size, timer) {
    console.log("temp");
    if (timer === undefined) timer = undefined; // Thiết lập giá trị mặc định nếu cần
    let container_noti = document.getElementsByClassName("container_noti")[0];
    switch (size) {
        case 'small':
            container_noti.classList.add("small");
            break;
        case 'medium':
            container_noti.classList.add("medium");
            break;
        case 'large':
            container_noti.classList.add("large");
            break;
    }
    let load = document.getElementsByClassName("box_loading")[0];
    load.classList.remove("display_none_loading");
    let overlay_menu = document.getElementsByClassName("overlay_menu")[0];
    overlay_menu.classList.remove("display_none_loading");
    if (type === 'text') {
        let eImage = document.getElementsByClassName("box_loading")[0];
        eImage.classList.add("display_none_loading");
    }
    if (type === 'image') {
        let eSpan = document.getElementById("titleId");
        eSpan.classList.add("display_none_loading");
    }
    if (text_cotent) {
        document.getElementsByClassName("container_noti")[0].classList.remove("display_none_loading");
        let title_span = document.getElementById("titleId");
        title_span.innerText = "";
        title_span.innerText = text_cotent;
        $('.content_header').text(text_title);
    }
    if (timer !== undefined) {
        setTimeout(function () {
            var isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
            var isWebview = /; wv/i.test(navigator.userAgent);
            sessionStorage.clear();
            if (isMobile && isWebview) {
                Android.nextScreen('1');
            } else {
                console.log("handleBack: web");
            }
        }, timer);
    }
}

function Click_Cont(id) {
    booking.cont = id;
}

function Click_District(id) {
    booking.cd = id;
}

async function Click_Area(id) {
    autoArea = false;
    booking.area = id;
    var response = await $.ajax({
        type: 'POST',
        url: '/Home/clickArea',
        data: { para: id },
        dataType: 'JSON',
        async: true,
        success: async function (data) {
            $("#provinceList").html(data);
        },
    });
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = response;
    return tempDiv.firstChild;
}

async function Click_Province(id) {
    autoProvince = false;
    booking.province = id;
    var response = await $.ajax({
        type: 'POST',
        url: '/Home/clickProvince',
        data: { para: id },
        dataType: 'JSON',
        async: true,
        success: async function (data) {
            $("#districtList").html(data);
        },
    });
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = response;
    return tempDiv.firstChild;
}