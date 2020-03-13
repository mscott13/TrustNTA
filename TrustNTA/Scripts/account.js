$(document).ready(function () {
    $("input[name='register-as']").change(function () {
        console.log($(this).val());

        $("#choose-company-container").toggleClass("company-hide");
        $("#company-associated-container").toggleClass("company-hide");
    });
});