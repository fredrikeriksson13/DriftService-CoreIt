//ToDO: Verifiering på email adress
// Telefon nummer numeriskt


function editProduct(contactID) {
    $.ajax({
        url: '/Contact/Edit/' + contactID, // The method name + paramater
        success: function (data) {
           
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $(function () { $('#editModal').modal(); });
        }
    });
}




//function isValidEmailAddress(emailAddress) {
//    var pattern = new RegExp(/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);

//  return pattern.test(emailAddress.val())
//};

$("#SelectedSms, #SelectedEmail").mouseleave(function () {

    if (!$("#SelectedSms").is(':checked') && !$("#SelectedEmail").is(':checked')) {
        $("#notificationErrorMessage span").text("Atlest one Notificationtype must be selected.");
    }
    else {
        $("#notificationErrorMessage span").text("");
    }

    if (($("#FirstName").val() == "") || ($("#LastName").val() == "") || ($("#Business").val() == "") ||  (!$("#SelectedSms").is(':checked') && !$("#SelectedEmail").is(':checked'))) {
        $("#btnSubmit").attr("disabled", true)
    }
    else {
        $("#btnSubmit").attr("disabled", false)
    }
});

$("#SelectedSms, #SelectedEmail, #FirstName, #LastName, #Business, #Email").keyup(function () {
    
    

        if (($("#FirstName").val() == "")) {
            $("#FirstNameErrorMessage span").text("Please enter Firtst name.");
        }
        else {
            $("#FirstNameErrorMessage span").text("");
        }
        if (($("#LastName").val() == "")) {
            $("#LastNameErrorMessage span").text("Please enter Last name.");
        }
        else {
            $("#LastNameErrorMessage span").text("");
        }
        if (($("#Business").val() == "")) {
            $("#BusinessErrorMessage span").text("Please enter company name.");
        }
        else {
            $("#BusinessErrorMessage span").text("");
        }
       
         if (($("#FirstName").val() == "") || ($("#LastName").val() == "") || ($("#Business").val() == "") || !$("#SelectedSms").is(':checked') && !$("#SelectedEmail").is(':checked')) {
            $("#btnSubmit").attr("disabled", true)
    }
    else {
        $("#btnSubmit").attr("disabled", false)
    }

   
});





//function isValidPhoneNumber(PhoneNumber) {
//    var pattern = new RegExp('^/+[0-9]*$');
//    return pattern.test(PhoneNumber);
//};

//function EditRow(Id) {

   
//    var row = document.getElementById("ContactTable").rows.namedItem(Id);
   
//    var firstNameCell = row.cells[0];
//    var lastNameCell = row.cells[1];
//    var companyCell = row.cells[2];
//    var emailCell = row.cells[4];
//    var phonenumberCell = row.cells[5];

//    $(firstNameCell).prop('contenteditable', true)
//    $(lastNameCell).prop('contenteditable', true)
//    $(companyCell).prop('contenteditable', true)
//    $(emailCell).prop('contenteditable', true)
//    $(phonenumberCell).prop('contenteditable', true)
//    //Gör row ej clickable, ändra färg till vit
   
   
    
//};
   
   
