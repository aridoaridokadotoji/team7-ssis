$(document).ready(function(){
	
    var action_dropbox = function (data, type, row, meta) {
        if (data == "Delivered") {
            return '<select  class="action form-control form-control-sm"><option value="-1">Action</option><option value="0">View Purchase Order details</option> <option value=1>View Related Delivery Orders</option></select>';

        }
        else if (data == "Partially Delivered") {
            return '<select  class=" action form-control form-control-sm"><option value="-1">Action</option><option value="0">View Purchase Order details</option> <option value=1>View Related Delivery Orders</option><option value=2 > Receive Goods</ option></select>';
        }
        else if (data == "Awaiting Delivery") {
            return '<select class="action form-control form-control-sm"><option value="-1">Action</option><option value="0">View Purchase Order details</option> <option value=2 > Receive Goods</ option></select>';
        }
        else
            return '<select class="action form-control form-control-sm"><option value="-1">Action</option><option value="0">View Purchase Order details</option> </select>';
        }
       
        
		var $table=$('#poTable');
	
		
		var datatbl = $table.DataTable(
            {
               
                language: {
                    search: "_INPUT_",
                    searchPlaceholder: "Search...",
                },
                ajax: {

                    url: "api/purchaseOrder/all",
                    dataSrc: ""    
                },
                
                columns: [
                    
                    {
                        data: "PurchaseOrderNo",
                        defaultContent: "<i>Not available</i>"
                    },
                    {
                        data: "SupplierName",
                        defaultContent: "<i>Not available</i>"
                    },
                    {
                        data: "CreatedDate",
                        defaultContent: "<i>Not available</i>"
                    },
                    {
                        data: "Status"  ,
                        defaultContent: "<i>Not available</i>"
                    },
                    {
                        data: "Status",
                        render: action_dropbox,
                        defaultContent: "<i>Not available</i>"
                    }
                    
                ],
                autowidth: true,
                select: "single",
                ordering: false,
               
                createdRow: function (row, data, dataIndex) {
                    if (data.Status == "Delivered") {
                        $('td', row).eq(3).addClass('delivered');
                    }
                    if (data.Status == "Partially Delivered") {
                        $('td', row).eq(3).addClass('partially-delivered');
                    }
                    if (data.Status == "Awaiting Delivery") {
                        $('td', row).eq(3).addClass('awaiting-delivery');
                    }
                    if (data.Status == "Cancelled") {
                        $('td', row).eq(3).addClass('cancel');
                    }
                },

                initComplete: function (){ // After DataTable initialized
                    
                    this.api().columns([3]).every(function () {
                        var column = this;
                        var select = $('<select multiple id="sel1" title="All Statuses" data-width="auto" data-style="btn-sm" class=" selectpicker  " ></select>')
                            .prependTo($('.dataTables_filter')) 
                        var download = $('<a id="downloadselected" class=" btn  btn-primary pull-left mr-3 btn-sm btn" href="#"><i class="fa fa-download" ></i>  Download PDF of Selected</a>').prependTo($('#poTable_length'))
                        var select = $('#sel1').on('change', function () {
                           
                            var val = $(this).val() + '';
               

                            column.search(val != '' ? '^' + val.split(',').join('$|^') + '$' : '', true, false).draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                        $('.selectpicker').selectpicker();  
                    }); // this.api function


                } //initComplete function 

               
        });

    $(document).on("click", "#downloadselected", function () {

        var data = $('#poTable').DataTable().rows({ selected: true }).data().toArray();
        if (data.length != 0) {
            ponum = data[0].PurchaseOrderNo;

            var form = document.createElement("form");
            var element1 = document.createElement("input");
            form.method = "POST";
            form.action = "/PurchaseOrder/downloadselectedpdf";

            element1.value = ponum;
            element1.name = "purchaseOrderNum";
            element1.type = "hidden";
            form.appendChild(element1);

            document.body.appendChild(form);

            form.submit();
        }
        else {
            alert("Please select a purchase order to download!");
        }
        

    });


    $('#poTable tbody').on('change', '.action', function (e) {

        var poNum = datatbl.row($(this).parents('tr')).data().PurchaseOrderNo;
        
        var value = Number($(this).val());
        var url;
        //url = $("#relDelOrdersUrl").val();
        //alert(url);

        if (value == 0) { url = $("#detailsUrl").val(); }
        else if (value == 1) { url = $("#relDelOrdersUrl").val(); }
        else if (value == 2) { url = $("#receiveGoodsUrl").val(); }
        else { url = ''; }

      

        var form = document.createElement("form");
        var element1 = document.createElement("input");
        form.method = "POST";
        form.action = url;

        element1.value = poNum;
        element1.name = "pon";
        element1.type = "hidden";
        form.appendChild(element1);

        document.body.appendChild(form);

        form.submit();


    });


    var simple_cancel = function (data, type, row, meta) {
        var poStatus = $("#poStatus").val();

        var html = '<a class="cancelPODbtn btn-default btn disabled" ><i class="fa fa-close"></i></a>';

        if (poStatus == "Partially Delivered") {
            if (data == "Awaiting Delivery") {
                html = '<a id="cancelPartially" class="cancelPODbtn btn-default btn"  ><i class="fa fa-close"></i></a>';
            }
        }
        return html;
        
    };

    $(document).on("click", "#cancelPartially", function () {

       // alert("Hi");
        var pNum = $("#purchaseOrderNo").val();
        var itemCode = poddatatbl.row($(this).parents('tr')).data().ItemCode;
       // alert(itemCode);

        $.ajax({

            type: "POST",
            url: "/PurchaseOrder/cancelPODetail",
            dataType: "json",
            data: JSON.stringify({ purchaseOrderNum: pNum , itemCode: itemCode }),
            contentType: "application/json",
            cache: true,
            success: function (data) {
                if (data.amount == 0) {
                    window.location.reload();
                }

                document.getElementById("amountLabel").innerHTML = "$ " + data.amount;
                poddatatbl.ajax.reload();

            }
        });


    });


    var podUrl = $("#purchaseOrderNo").val();

   
    //purchase order details table
    var $podtable = $('#poDetailTable');
    var poddatatbl = $podtable.DataTable(
        {
            dom: "t",
            ajax: {

                url: '/api/purchaseOrder/details/' + podUrl,
                dataSrc: ''
            },

            columns: [
                { data: "ItemCode" },
                {
                    data: "Description",
                    defaultContent: "<i>Not available</i>"
                },
                {
                    data: "QuantityOrdered",
                    defaultContent: "<i>Not available</i>"
                },
                {
                    data: "UnitPrice",
                    render: $.fn.dataTable.render.number(',', '.', 2, '$'),
                    defaultContent: "<i>Not available</i>"
                },
                {
                    data: "Amount",
                    render: $.fn.dataTable.render.number(',', '.', 2, '$')
                },
                {
                    data: "ReceivedQuantity"
                },
                {
                    data: "RemainingQuantity"
                },
                {
                    data: "Status"
                },
                {
                    data: "Status",
                    render: simple_cancel
                }
               
            ],
            autowidth: true,
            select: "single",
            createdRow: function (row, data, dataIndex) {
                if (data.Status == "Delivered") {
                    $('td', row).eq(7).addClass('delivered');
                }
                if (data.Status == "Partially Delivered") {
                    $('td', row).eq(7).addClass('partially-delivered');
                }
                if (data.Status == "Awaiting Delivery") {
                    $('td', row).eq(7).addClass('awaiting-delivery');
                }
                if (data.Status == "Cancelled") {
                    $('td', row).eq(7).addClass('cancel');
                }
            }

        });



    //purchase order detail awaiting table
    
    var $podAwaitingtable = $('#poDetailAwaitingTable');
    var podAwaitingdatatbl = $podAwaitingtable.DataTable(
        {
            dom:"t",

            ajax: {

                url: '/api/purchaseOrder/details/' + podUrl,
                dataSrc: ''
            },

            columns: [
                { data: "ItemCode" },
                {
                    data: "Description",
                    defaultContent: "<i>Not available</i>"
                },
                {
                    data: "QuantityOrdered",
                    render: function (data, type, row, meta) {
                        var html = '<input class="editPO" id="' + row.ItemCode + '"type="number" ' +
                            'value="' + data + '"/>';
                        return html;
                    } 

                },
                {
                    data: "UnitPrice",
                    render: $.fn.dataTable.render.number(',', '.', 2, '$')
                    
                },
                {
                    data: "Amount",
                    render: $.fn.dataTable.render.number(',', '.', 2, '$')
                },
                {
                    data: "Status"
                },
                {
                    defaultContent: '<a class="deletebtn btn-default btn " href="#" ><i class="fa fa-close"></i></a>'  
                }
                
            ],
            autowidth: true,
            select: "single",

            createdRow: function (row, data, dataIndex) {
                if (data.Status == "Delivered") {
                    $('td', row).eq(5).addClass('delivered');
                }
                if (data.Status == "Partially Delivered") {
                    $('td', row).eq(5).addClass('partially-delivered');
                }
                if (data.Status == "Awaiting Delivery") {
                    $('td', row).eq(5).addClass('awaiting-delivery');
                }
            }

        });

    $(document).on("change", ".editPO", function () {

        var cell = podAwaitingdatatbl.cell(this.parentElement);
        cell.data($(this).val()).draw();

        //alert(this.id);
        var rowIdx = document.getElementById(this.id).parentElement.parentElement;

        //works in console
        // alert($('#generatePoTable').DataTable().rows().data()[0].TotalPrice);
        //var quantity = parseInt(cell.data());

        //alert(row(generatePOTbl.this.parentElement));

        var quantity = document.getElementById(this.id).value;
        var unitPrice = $('#poDetailAwaitingTable').DataTable().row(document.getElementById(this.id).parentElement.parentElement).data().UnitPrice;



        // alert("AMOUNT "+generatePOTbl.row($(this.id).closest('tr')).data().UnitPriceDecimal);
        // var amount = parseFloat(generatePOTbl.row(this.parentElement).data().UnitPriceDecimal);

        var totalAmount = parseInt(quantity) * parseFloat(unitPrice);
        podAwaitingdatatbl.cell(rowIdx, 4).data(totalAmount).draw();




        //.draw();
        // document.getElementById("")


    });




    $('#poDetailAwaitingTable tbody').on('click', '.deletebtn', function (e){

        var itemCode = podAwaitingdatatbl.row($(this).parents('tr')).data().ItemCode;
        var pNum = $("#purchaseOrderNo").val();

        $.ajax({

            type: "POST",
            url: "/PurchaseOrder/delete",
            dataType: "json",
            data: JSON.stringify({ purchaseOrderNum: pNum, itemCode: itemCode}),
            contentType: "application/json",
            cache: true,
            success: function (data) {
                if (data.amount==0) {
                    window.location.reload();
                }
                document.getElementById("amountLabel").innerHTML = "$ " + data.amount;
                podAwaitingdatatbl.ajax.reload();
            }
    });
        
        
    });


    $('#cancelpurchaseorder').on('click', function (e) {
        var pNum = $("#purchaseOrderNo").val();

        $.ajax({

            type: "POST",
            url: "/PurchaseOrder/cancelPO",
            dataType: "json",
            data: JSON.stringify({ purchaseOrderNum: pNum }),
            contentType: "application/json",
            cache: true,
            success: function (result) {
                if (result.status == "Cancelled") {
                    window.location.reload();
                }
                
            }
        });


    });


    $('#viewRelatedDel').on('click', function (e) {
       // alert("Hi");
        var pNum = $("#purchaseOrderNo").val();
        var url = $("#viewRelDelUrl").val();

       // alert(pNum);
       // alert(url);

        var form = document.createElement("form");
        var element1 = document.createElement("input");
        form.method = "POST";
        form.id = "myform";
        form.action = url;

        element1.value = pNum;
        element1.name = "pon";
        element1.type = "hidden";
        form.appendChild(element1);
        document.body.appendChild(form);

        //setTimeout(function () { form.submit(); }, 40000);
        form.submit();


    });

    $('#backBtn').on('click', function (e) {
        document.location.href = '/PurchaseOrder';
    });


    $('.savebutton').on('click', function (e) {

        // alert("in save");

        var item = podAwaitingdatatbl.rows().data().toArray();
        var flag = 0;
        var updateArr = new Array();

        for (i = 0; i < item.length; i++) {

            if (item[i].QuantityOrdered <= 0)
            {
                flag = 1;
                break;
            }

            var o = {
                "ItemCode": item[i].ItemCode,
                "QuantityOrdered": item[i].QuantityOrdered,
                "PurchaseOrderNo": podUrl
            }

            updateArr.push(o);

        }

        if (flag == 0) {
            $.ajax({

                type: "POST",
                url: "/purchaseOrder/update",
                dataType: "json",
                data: JSON.stringify(updateArr),
                contentType: "application/json",
                cache: true,
                success: function (result) {

                    document.getElementById("amountLabel").innerHTML = "$ " + result.amount;
                    alert("Quantity has been updated!");
                    podAwaitingdatatbl.ajax.reload();

                }
            });

        }

        else if (flag == 1) {
            alert("Please enter valid quantity");
        }
    });


    $(document).on("change", ".qty", function () {

        var cell = podAwaitingdatatbl.cell(this.parentElement);
        // assign the cell with the value from the <input> element 
        cell.data($(this).val()).draw();
    });

        

    

   
                       
});
