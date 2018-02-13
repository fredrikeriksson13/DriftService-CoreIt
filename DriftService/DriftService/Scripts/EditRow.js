


function editProduct(contactID) {
    $.ajax({
        url: '/Contact/Edit/' + contactID, // The method name + paramater
        success: function (data) {
           
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
            $(function () { $('#editModal').modal(); });
        }
    });
}


$("#FirstName").keyup(function () {
    var txtFirs = (this).val().lenght;
    if (txtFirs == 0) {
        $("#btnSubmit").attr("disabled", true);
    }
});  

function Validation(){
   
};







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
   
   
