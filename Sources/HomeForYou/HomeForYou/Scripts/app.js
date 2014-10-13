$(document).ready(function () {
    init();
    imagePreview();
    imagePreview1();
});

function GuiThongTin() {
    var sibling = $(".booking-item").siblings(".roomMultiRoomPrice");
    var sophong = $("select", sibling).val();
    if (sophong == "0") alert("Vui lòng chọn số phòng muốn đặt.");
    else {
        var madeal = $(".madeal", sibling).val();
        var ngaynhan = $("#from_datealt").val();
        var ngaytra = $("#to_datealt").val();
        var url = "../../ThanhToan/GuiThongTin/?madeal=" + madeal + "&sophong=" + sophong + "&ngaynhan='" + ngaynhan + "'&ngaytra='" + ngaytra + "'";
        window.location.hash = "";
        window.location = url;
    }
};

// Configuration of the x and y offsets
this.imagePreview = function () {
    xOffset = -20;
    yOffset = 40;
    this.imagePreview1 = function () {
        xOffset = -20;
        yOffset = 40;

		$("a.preview").hover(function (e) {
			this.t = this.title;
			this.title = "";
			var c = (this.t != "") ? "<br/>" + this.t : "";
			$("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='width: 250px; height: 250px;'/>" + c + "</p>");
			$("#preview")
					.css("top", (e.pageY - xOffset) + "px")
					.css("left", (e.pageX + yOffset) + "px")
					.fadeIn("slow");
		},

			function () {
				this.title = this.t;
				$("#preview").remove();

			});

		$("a.preview").mousemove(function (e) {
			$("#preview")
					.css("top", (e.pageY - xOffset) + "px")
					.css("left", (e.pageX + yOffset) + "px");
		});
	};
	//hinh tren chitiet
    $("a.preview1").hover(function (e) {
                 this.t = this.title;
                 this.title = "";
                 var c = (this.t != "") ? "<br/>" + this.t : "";
                 $("body").append("<p id='preview1'><img src='" + this.href + "' alt='Image preview' style='width: 250px; height: 150px;'/>" + c + "</p>");
                 $("#preview1")
                .css("top", (e.pageY - xOffset) + "px")
                .css("left", (e.pageX + yOffset) + "px")
                .fadeIn("slow");
             },

        function () {
            this.title = this.t;
            $("#preview1").remove();

        });

             $("a.preview1").mousemove(function (e) {
                 $("#preview1")
                .css("top", (e.pageY - xOffset) + "px")
                .css("left", (e.pageX + yOffset) + "px");
             });
         };
//ket thuc hinh chi tiet

function init() {
    $("table .delete-row").click(function () {
        if (!confirm("Are you sure you want to Delete?")) return false;
        var ele = $(this);
        $.get(ele.attr('href'), "", function () {
            ele.parents("tr").fadeOut('slow');
        });
        return false; // khong chuyen trang
    });

    $("#sidebar .danhmuc").click(function () {
        $(".danhmuc-items").toggle("slow");
        return false;
    });

    $("#sidebar .thongke").click(function () {
        $(".thongke-items").toggle("slow");
        return false;
    });

    $("#sidebar .khuyenmai").click(function () {
        $(".khuyenmai-items").toggle("slow");
        return false;
    });

    $(".alert").click(function () {
        $(this).hide();
    });

    $("#start_date").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        numberOfMonths: 1,
        dateFormat: 'd/m/yy',
        altField: "#start_datealt",
        altFormat: "mm/dd/yy",
        onClose: function (selectedDate) {
            $("#end_date").datepicker("option", "minDate", selectedDate);
        }
    });
    $("#end_date").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        numberOfMonths: 1,
        dateFormat: 'd/m/yy',
        altField: "#end_datealt",
        altFormat: "mm/dd/yy",
        onClose: function (selectedDate) {
            $("#start_date").datepicker("option", "maxDate", selectedDate);
        }
    });

    $("#from_date").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        numberOfMonths: 1,
        dateFormat: 'd/m/yy',
        altField: "#from_datealt",
        altFormat: "mm/dd/yy",
        onClose: function (selectedDate) {
            $("#to_date").datepicker("option", "minDate", selectedDate);
        }
    });
    $("#to_date").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        changeYear: true,
        dateFormat: 'd/m/yy',
        altField: "#to_datealt",
        altFormat: "mm/dd/yy",
        onClose: function (selectedDate) {
            $("#from_date").datepicker("option", "maxDate", selectedDate);

        }
    });
        
    $("#datepicker").datepicker({
        dateFormat: 'd/m/yy',
        altField: '#alt-datepicker',
        changeMonth: true,
        changeYear: true,
        //altField: ".date_alternate",
        altFormat: "mm/dd/yy"
    });

    $(".monthPicker").datepicker({
        dateFormat: 'mm-yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        onClose: function (dateText, inst) {
            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            $(this).val($.datepicker.formatDate('mm-yy', new Date(year, month, 1)));
            //$(".monthPicker-kt").datepicker("option", "minMonth", month);
        }
    });

    $(".monthPicker").focus(function () {
        $(".ui-datepicker-calendar").hide();
        $("#ui-datepicker-div").position({
            my: "center top",
            at: "center bottom",
            of: $(this)
        });
    });

    $(".monthPicker-kt").datepicker({
        dateFormat: 'mm-yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        onClose: function (dateText, inst) {
            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            $(this).val($.datepicker.formatDate('mm-yy', new Date(year, month, 1)));
            //$(".monthPicker-bd").datepicker("option", "maxMonth", month);
        }
    });

    $(".monthPicker-kt").focus(function () {
        $(".ui-datepicker-calendar").hide();
        $("#ui-datepicker-div").position({
            my: "center top",
            at: "center bottom",
            of: $(this)
        });
    });

    $(".container .country-list #country_list").change(function () { // get city list by id of a country
        $('.container .city-list #city_list option').remove();
        $('.container .area-list #area_list option').remove();
        $('.container .hotel-list #hotel_list option').remove();
        $('.container .city-list #city_list').append(
            "<option value='0'>Thành phố</option>"
            );
        $('.container .area-list #area_list').append(
            "<option value='0'>Vùng</option>"
            );
        $('.container .hotel-list #hotel_list').append(
            "<option value='0'>Khách sạn</option>"
            );
        var country_code = $(this).val();
        if (country_code != 0 && country_code != null) {
            $.ajax({
                url: '/back/DealKS/DanhSach_ThanhPho',
                type: "POST",
                data: {
                    ma_quoc_gia: country_code
                },
                dataType: "json",
                success: function (result) {
                    if (result.length != 0) {
                        $.each(result, function (i, item) {
                            $('.container .city-list #city_list').append(
                                "<option value='" + item.ma_tp + "'>" + item.ten_tp + "</option>"
                                );
                        });
                    }
                }
            });
        }
    });
    $(".container .city-list #city_list").change(function () {  // get area list by id of a city
        $('.container .area-list #area_list option').remove();
        $('.container .hotel-list #hotel_list option').remove();
        $('.container .area-list #area_list').append(
            "<option value='0'>Vùng</option>"
            );
        $('.container .hotel-list #hotel_list').append(
            "<option value='0'>Khách sạn</option>"
            );
        var city_code = $(this).val();
        if (city_code != 0 && city_code != null) {
            $.ajax({
                url: '/back/DealKS/DanhSach_Vung',
                type: "POST",
                data: {
                    ma_thanh_pho: city_code
                },
                dataType: "json",
                success: function (result) {
                    if (result.length != 0) {
                        $.each(result, function (i, item) {
                            $('.container .area-list #area_list').append(
                                "<option value='" + item.ma_vung + "'>" + item.ten_vung + "</option>"
                                );
                        });
                    }
                }
            });
        }
    });

//    $(".container .city-list #city_list").change(function () {  // get area list by id of a city
//        $('.container .area-list #area_list option').remove();
//        $('.container .hotel-list #hotel_list option').remove();
//        $('.container .area-list #area_list').append(
//            "<option value='0'>Vùng</option>"
//            );
//        var city_code = $(this).val();
//        if (city_code != 0 && city_code != null) {
//            $.ajax({
//                url: '/back/KhachSan/DanhSach_Vung',
//                type: "POST",
//                data: {
//                    ma_thanh_pho: city_code
//                },
//                dataType: "json",
//                success: function (result) {
//                    if (result.length != 0) {
//                        $.each(result, function (i, item) {
//                            $('.container .area-list #area_list').append(
//                                "<option value='" + item.ma_vung + "'>" + item.ten_vung + "</option>"
//                                );
//                        });
//                    }
//                }
//            });
//        }
//    });

    $(".container .area-list #area_list").change(function () { // get hotel list by id of a area
        $('.container .hotel-list #hotel_list option').remove();
        $('.container .hotel-list #hotel_list').append(
            "<option value='0'>Khách sạn</option>"
            );
        var area_code = $(this).val();
        if (area_code != 0 && area_code != null) {
            $.ajax({
                url: '/back/DealKS/DanhSach_KhachSan',
                type: "POST",
                data: {
                    ma_vung: area_code
                },
                dataType: "json",
                success: function (result) {
                    if (result.length != 0) {
                        $.each(result, function (i, item) {
                            $('.container .hotel-list #hotel_list').append(
                                "<option value='" + item.ma_ks + "'>" + item.ten_ks + "</option>"
                                );
                        });
                    }
                }
            });
        }
    });
    $(".add-deal #khachsan_list").change(function () {
        console.log("aaa");
        var hotel_code = $(this).val();
        if (hotel_code != 0 && hotel_code != null) {
            $.ajax({
                url: '/back/DealKS/DanhSach_Phong',
                type: "POST",
                data: {
                    ma_ks: hotel_code
                },
                dataType: "json",
                success: function (result) {
                    if (result.length != 0) {
                        $.each(result, function (i, item) {
                            $('.add-deal #phong_list').append(
                                "<option value='" + item.ma_phong + "'>" + item.ten_phong + "</option>"
                                );
                        });
                    }
                }
            });
        }
    });
    //front/index
	//Mido's rating
    $(".icon_thumb_up_btn").click(function () {
        var nowrap = $(this).parents(".nowrap");
        var removeFirstBorder = $(this).parents(".removeFirstBorder");
        var hotel_code = $("#ma_ks", nowrap).val();
        $(".icon_thumb_down_btn").removeAttr("disabled");  
        $(".icon_thumb_up_btn").attr("disabled", "disabled");    
        //alert(up);
        var vote = 1;
        console.log(hotel_code);
        $.ajax({
            url: '/front/KhachSan/Rating',
            type: "POST",
            data: {
                ma_ks: hotel_code,
                vote: vote
            },
            dataType: "json",
            success: function (result) {
                if (result <= 5) {
                    $(".review_overview_switch").html("<strong>Khá tốt, " + result + " Điểm</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Khá tốt, " + result + " Điểm</strong>");
                }
                else if (result > 5 && result < 8) {
                    $(".review_overview_switch").html("<strong>Tốt, " + result + " Điểm</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Tốt, " + result + " Điểm</strong>");
                }
                else {
                    $(".review_overview_switch").html("<strong>Rất tốt, " + result + " Điểm</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Rất tốt, " + result + " Điểm</strong>");
                }
                $(".thumb-up-message", nowrap).fadeIn(100);
                $(".thumb-up-message", nowrap).fadeOut(800);
            }
        });
    });
    $(".icon_thumb_down_btn").click(function () {
        var nowrap = $(this).parents(".nowrap");
        var hotel_code = $("#ma_ks", nowrap).val();
        var removeFirstBorder = $(this).parents(".removeFirstBorder");
        $(".icon_thumb_down_btn").attr("disabled", "disabled");
        $(".icon_thumb_up_btn").removeAttr("disabled");      
        var vote = -1;
        $.ajax({
            url: '/front/KhachSan/Rating',
            type: "POST",
            data: {
                ma_ks: hotel_code,
                vote: vote
            },
            dataType: "json",
            success: function (result) {
                if (result <= 5) {
                    $(".review_overview_switch").html("<strong>Khá tốt, " + result + " Điểm</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Khá tốt, " + result + " Điểm</strong>");
                }
                else if (result > 5 && result < 8) {
                    $(".review_overview_switch").html("<strong>Tốt, " + result + "</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Tốt, " + result + " Điểm</strong>");
                }
                else {
                    $(".review_overview_switch").html("<strong>Rất tốt, " + result + " Điểm</strong>");
                    $(".rating .average", removeFirstBorder).html("<strong>Rất tốt, " + result + " Điểm</strong>");
                }
                $(".thumb-down-message", nowrap).fadeIn(100);
                $(".thumb-down-message", nowrap).fadeOut(800);
            }
        });
    });
	//end Mido's rating
    $(".deal-search-form #find_deal").click(function () { // get deals of a hotel
        $('.roomArea').hide();
        $('#maxotel_rooms #room_availability_container').empty();
        var from_date = $(".deal-search-form #from_datealt").val();
        var to_date = $(".deal-search-form #to_datealt").val();
        if (from_date == "" || to_date == "") {
            $('#noroom').hide();
            $("#date_loi").show(); return false;
            //alert("Vui lòng chọn ngày nhận và ngày trả phòng!");
        }
        else {
            $("#date_loi").hide();
            var id_ks = $("#idks").val();
            $.ajax({
                url: '/front/Deal/TimKiemDeal',
                type: "POST",
                data: {
                    tu_ngay: from_date,
                    den_ngay: to_date,
                    id_ks: id_ks
                },
                dataType: "json",
                success: function (result) {
                    if (result.length != 0) {
                        $('#noroom').hide();
                        $.each(result, function (i, item) {
                            // add so luong phong
                            var s = "";
                            for (var j = 0; j <= item.so_luong_phong; j = j + 1) {
                                s = s + "<option value='" + j + "'>" + j + "</option>";
                            }
                            $('#maxotel_rooms #room_availability_container').append(
                                    "<tr class='deal" + item.ma_deal + " maintr' >"
                                    + "<td class='roomType' rowspan='2'>"
                                    + "<div>"
                                    + "<span>"
                                    + "<a href='#RD' class='smll_roomphoto jqrt jq_tooltippex'>"
                                    + "<img src='/Content/img/10576864.jpg' class='round6' alt='Phòng Cổ điển - Giường cỡ King' height='60' width='60'>"
                                    + "</a>"
                                    + "</span>"
                                    + "<span style='display: block'>"
                                    + "<a class='togglelink jqrt' title=''>" + item.ten_phong
                                    + "</a>"
                                    + "</span>"
                                    + "<div class='small'>"
                                    + "Giá áp dụng cho 1 phòng <br>"
                                    + "<div class='incExcInPriceNew'>"
                                    + "<span class='incExcEmphasize'>Bao gồm</span> trong giá phòng: 20 % Thuế GTGT."
                                    + "</div>"
                                    + "</div>"
                                    + "</div>"
                                    + "</td>"
                                    + "<td class='roomMaxPersons figure bb'>"
                                    + "<div class='roomDefaultUse'>"
                                    + "<img width='43' height='10' alt='' class='occsprite max2 jq_tooltip' src='/Content/img/transparent.png'>"
                                    + "</div>"
                                    + "</td>"
                                    + "<td class='roomPrice figure bb'>"
                                    + "<div id='room_id_" + item.ma_phong + "' class='roomDefaultUse  flash_deal_block'>"
                                    + "<strong  class='click_change_currency jq_tooltip toggle_price_per_night_or_stay'>" + item.gia + " VNĐ "
                                    + "</strong> <br>"
                                    + "</div>"
                                    + "</td>"
                                    + "<td class='roomMultiRoomPrice bb'>"
                                    + "<div class='roomDefaultUse'>"
                                    + "<input type='hidden' class='madeal' value='" + item.ma_deal + "'/>"
                                    + "<select id='sophong" + item.ma_deal + "' name='sophong' class='b_available_multi_room_price limited_rooms'>"
                                    + s
                                    + "</select>"
                                    + "</div>"
                                    + "</td>"

                                    + "<td class='booking-item'>"
                                    + "<div style='display: block; margin-left: 0px; top: 0px; position: relative; width: 69px;' class='bookNowWrap'>"


                                    + "<button class='booking-deal btn btn-success' onclick='GuiThongTin();'>Đặt ngay</button>"
                                    + "</div>"
                                    + "<tr class='room_loop_counter extendedRow'>"
                                    + "<td colspan='3'>"
                                    + "</td>"
                                    + "</tr>"

                                    );
                        });
                        $('.roomArea').show();
                    }
                    else 
                        $('#noroom').show();
                }
            });
        }
    });
	//Nhơn
    $("#bookForm #option1").change(function () {
        var element = $(this).attr("checked");
        if (element == "checked") {
            $("#div_group2").hide();
            $("#div_group1").show();

        }
    }); $("#bookForm #option2").change(function () {
        var element = $(this).attr("checked");
        if (element == "checked") {
            $("#div_group1").hide();
            $("#div_group2").show();
        }
    });

    //Nhơn
    //Heo
    $("#exp_layout #thanhtoan").change(function () {
        console.log("abcd");
        var thanhtoan = $(this).val();
        switch (thanhtoan) {
            case "1":
                $("#tienmat").show();
                $("#tindung").hide();
                $("#ATM").hide();
                $("#pt_loi").hide();
                break;
            case "2":
                $("#tindung").show();
                $("#tienmat").hide();
                $("#ATM").hide();
                $("#pt_loi").hide();
                break;
            case "3":
                $("#ATM").show();
                $("#tindung").hide();
                $("#tienmat").hide();
                $("#pt_loi").hide();
                break; default: $("#ATM").hide();
                $("#tindung").hide();
                $("#tienmat").hide();
                $("#pt_loi").hide();
                break;
        }

    }
    );
    //Heo

    //An's

    $("#_sothe").focusout(function () {
        //alert("a");
        if ($("#_sothe").val() == "") {
            $("#thetd_loi").show();
        }
        else $("#thetd_loi").hide();
    });
    $("#_macvc").focusout(function () {
        if ($("#_macvc").val() == "") {
            $("#cvc_loi").show();
        }
        else $("#cvc_loi").hide();
    });


    //end An's
}

