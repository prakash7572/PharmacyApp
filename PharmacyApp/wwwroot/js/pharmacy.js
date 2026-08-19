$(document).ready(function () {

    loadMedicines();
    loadSales();


    $("#btnAddMedicine").click(function () {
        $("#medicineForm").slideDown();
    });


    $("#btnCancel").click(function () {

        $("#medicineForm").slideUp();
        $("#addMedicineForm")[0].reset();

    });


    $("#searchBox").keyup(function () {

        var searchText = $(this).val();

        loadMedicines(searchText);

    });


    $("#addMedicineForm").submit(function (e) {
        e.preventDefault();
        addMedicine();

    });

});


function loadMedicines(searchText = "") {

    $.ajax({
        url: "/api/medicines",
        type: "GET",
        data: {
            search: searchText
        },
        success: function (medicines) {
            $("#medicineTableBody").empty();
            $.each(medicines, function (index, medicine) {
                var expiryDate =
                    new Date(medicine.expiryDate);
                var today = new Date();
                var difference =
                    expiryDate.getTime() - today.getTime();
                var daysToExpiry =
                    Math.ceil(
                        difference /
                        (1000 * 60 * 60 * 24)
                    );

                var rowClass = "";

                // Expiry less than 30 days
                if (daysToExpiry < 30) {
                    rowClass = "table-danger";
                }

                // Quantity less than 10
                else if (medicine.quantity < 10) {
                    rowClass = "table-warning";
                }


                var row = `
                    <tr class="${rowClass}">

                        <td>
                            ${medicine.fullName}
                        </td>

                        <td>
                            ${medicine.brand}
                        </td>

                        <td>
                            ${expiryDate.toLocaleDateString()}
                        </td>

                        <td>
                            ${medicine.quantity}
                        </td>

                        <td>
                            ₹${Number(medicine.price).toFixed(2)}
                        </td>

                        <td>

                            <button
                                class="btn btn-success btn-sm"
                                onclick="sellMedicine(${medicine.id})">

                                Sell

                            </button>

                        </td>

                    </tr>
                `;


                $("#medicineTableBody")
                    .append(row);

            });

        },

        error: function () {

            alert("Unable to load medicines.");

        }

    });

}

function addMedicine() {

    var medicine = {

        fullName: $("#fullName").val(),

        notes: $("#notes").val(),

        expiryDate: $("#expiryDate").val(),

        quantity: parseInt(
            $("#quantity").val()
        ),

        price: parseFloat(
            $("#price").val()
        ),

        brand: $("#brand").val()

    };


    $.ajax({

        url: "/api/medicines",

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(medicine),

        success: function () {

            alert(
                "Medicine added successfully!"
            );

            $("#addMedicineForm")[0].reset();

            $("#medicineForm").slideUp();

            loadMedicines();

        },

        error: function (xhr) {

            console.log(xhr);

            alert(
                "Unable to add medicine."
            );

        }

    });

}

function sellMedicine(medicineId) {

    var quantity = prompt(
        "Enter quantity to sell:"
    );


    if (quantity === null) {

        return;

    }


    quantity = parseInt(quantity);


    if (isNaN(quantity) || quantity <= 0) {

        alert(
            "Please enter a valid quantity."
        );

        return;

    }


    var sale = {

        medicineId: medicineId,

        quantity: quantity

    };


    $.ajax({

        url: "/api/sales",

        type: "POST",

        contentType: "application/json",

        data: JSON.stringify(sale),

        success: function () {

            alert(
                "Sale completed successfully!"
            );

            loadMedicines();

            loadSales();

        },

        error: function (xhr) {

            alert(
                xhr.responseText ||
                "Unable to complete sale."
            );

        }

    });

}

function loadSales() {

    $.ajax({

        url: "/api/sales",

        type: "GET",

        success: function (sales) {

            $("#salesTableBody").empty();


            $.each(sales, function (index, sale) {

                var saleDate =
                    new Date(sale.saleDate);


                var row = `

                    <tr>

                        <td>
                            ${sale.medicineName}
                        </td>

                        <td>
                            ${sale.quantity}
                        </td>

                        <td>
                            ₹${Number(
                    sale.unitPrice
                ).toFixed(2)}
                        </td>

                        <td>
                            ₹${Number(
                    sale.totalPrice
                ).toFixed(2)}
                        </td>

                        <td>
                            ${saleDate.toLocaleString()}
                        </td>

                    </tr>

                `;


                $("#salesTableBody")
                    .append(row);

            });

        },

        error: function () {

            console.log(
                "Unable to load sales."
            );

        }

    });

}