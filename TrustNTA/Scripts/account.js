$(document).ready(function () {
    $("input[name='register-as']").change(function () {
        console.log($(this).val());

        $("#choose-company-container").toggleClass("company-hide");
        $("#company-associated-container").toggleClass("company-hide");
    });

    $("#btn-graduate-signin").click(function () {
        var graduateUsername = $("#graduate-identifier").val();
        var graduatePassword = $("#graduate-password").val();
        var json = JSON.stringify({ "username": graduateUsername, "password": graduatePassword });

        if (graduateUsername.length > 0 && graduatePassword.length > 0) {
            $.ajax({
                url: "/account/seeker-login",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: json,
                success: function (result) {
                    if (result.status === "user_authenticated") {
                        window.location.href = "/graduate/jobmanagement";
                    }
                    else {
                        alert("Password is incorrect, try again.");
                    }
                },
                error: function (result) {
                    alert(result);
                }
            });
        }
    });

    $("#btn-employer-login").click(function () {
        var employerUsername = $("#employer-username").val();
        var employerPassword = $("#employer-password").val();
        var json = JSON.stringify({ "username": employerUsername, "password": employerPassword });

        if (employerUsername.length > 0 && employerPassword.length > 0) {
            $.ajax({
                url: "/account/employer-login",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: json,
                success: function (result) {
                    if (result.status === "user_authenticated") {
                        window.location.href = "/employer/index";
                    }
                    else {
                        alert("Password is incorrect, try again.");
                    }
                },
                error: function (result) {
                    alert(result);
                }
            });
        }
    });

    $("#btn-register-employer").click(function () {
        var firstName = $("#emp_firstName").val();
        var lastName = $("#emp_lastName").val();
        var companyOptional = $("#emp_companyOptional").val();
        var companyAssociated = $("#company-selector").find(":selected").val();
        var address = $("#emp_address").val();
        var email = $("#emp_email").val();
        var telephone = $("#emp_telephone").val();
        var username = $("#emp_username").val();
        var password = $("#emp_password").val();
        var passwordConfirm = $("#emp_passwordConfirm").val();
        var accountType = $("input[name='register-as']:checked").val();
        

        if (firstName !== "" && lastName !== "" && address !== "" && email !== "" && telephone !== "" && username !== "" && accountType !== "") {
            if (password.length > 5 && password === passwordConfirm) {

                var json = "";
                var valid = false;

                if (accountType === "COMPANY" && companyAssociated !== "0") {
                    if (companyAssociated !== "0") {
                        json = JSON.stringify({
                            "accountType": accountType,
                            "firstName": firstName,
                            "lastName": lastName,
                            "companyAssociated": companyAssociated,
                            "companyOptional": "",
                            "address": address,
                            "emailAddress": email,
                            "telephone": telephone,
                            "username": username,
                            "password": password
                        });
                        valid = true;
                    }
                    else
                    {
                        alert("Choose a company from the dropdown or add a company if it is not listed");
                    }
                }
                else if (accountType === "INDIVIDUAL")
                {
                    json = JSON.stringify({
                        "accountType": accountType,
                        "firstName": firstName,
                        "lastName": lastName,
                        "companyAssociated": "0",
                        "companyOptional": companyOptional,
                        "address": address,
                        "emailAddress": email,
                        "telephone": telephone,
                        "username": username,
                        "password": password
                    });
                    valid = true;
                }

                if (valid)
                {
                    $.ajax({
                        url: "/account/register-employer",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        data: json,
                        success: function (result) {
                            if (result.status === "user_created")
                            {
                                $("#reg-success-modal").modal({ backdrop: 'static', keyboard: false });
                                console.log("user created");
                                //display message: account created, click here to login
                            }
                        },
                        error: function (result) {
                            console.log(result);
                            alert("Could not create user.");
                        }
                    });
                }
            }
            else
            {
                alert("Passwords must be at least 6 characters in length. Please supply a password and ensure that it matches");
            }
        }
        else
        {
            alert("Please fill out the required fields");
        }
    });

    $("#btn-add-company").click(function () {
        $("#add-company-modal").modal("show");
    });

    $("#set-as-company").click(function () {
        var companyName = $("#add_companyName").val();
        var companyAddress = $("#add_companyAddress").val();

        if (companyName !== "" && companyAddress !== "") {
            json = JSON.stringify({ "companyName": companyName, "address": companyAddress });

            $.ajax({
                url: "/account/add-company",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: json,
                success: function (result) {
                    if (result.status === "company_added") {
                        var option = new Option($("#add_companyName").val(), result.companyId, true, true);
                        $("#company-selector").append(option);
                        $("#add-company-modal").modal("hide");

                        $("#add_companyName").val('');
                        $("#add_companyAddress").val('');
                    }
                },
                error: function (result) {
                    alert("Company was not added, an error occurred");
                }
            });
        }
        else
        {
            alert("All fields are required");
        }
    });
});