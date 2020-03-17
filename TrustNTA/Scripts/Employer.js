$(document).ready(function () {
    var graduatesTable = $("#graduates-listing").DataTable({
        "pageLength": 50
    });

    var vacancyTable = $("#vacancy-listing").DataTable({
        "pageLength": 50
    });

    $("#btn-create-jpost").click(function () {
        window.location.href = "/employer/jobmanagement?intent=create";
    });

                                                                    
    $("#btn-create-jpost2").click(function () {
        $("#create-vacancy-modal").modal("show");
    });

    $("#btn-close-vacancy-modal").click(function () {
        ClearJobCreateFields();
    });

    $("#btn-post-vacancy").click(function () {
        var jobPostName = $("#vacancy-job-post-name").val();
        var jobType = $("#job-selector option:selected").val();
        var _jobLocations = $("#locations-selector option:selected");
        var startDate = $("#start-date").attr("data-dateraw");
        var endDate = $("#end-date").val();

        var jobLocations = [];
        $(_jobLocations).each(function () {
            var location = {};
            location.locationId = $(this).val();
            location.locationName = $(this).text();
            jobLocations.push(location);
        });

        if (jobType !== "0")
        {
            var json = JSON.stringify({ "jobType": jobType, "jobTitle": jobPostName, "startDate": startDate, "endDate": endDate, "locations": jobLocations });
            $.ajax({
                url: "/employer/add-vacancy",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: json,
                success: function (result) {
                    if (result.status === "vacancy_created") {
                        vacancyTable.row.add([
                            result.vacancy.jobTitle,
                            result.vacancy.html_jobType,
                            result.vacancy.html_locations,
                            result.vacancy.html_startDate,
                            result.vacancy.html_endDate,
                            result.vacancy.html_vacancyStatus,
                            "--",
                            "<i data-vid='" + result.vacancy.employerVacancyId + "' class='fas fa-edit btn-edit'></i>",
                            "<i data-vid='" + result.vacancy.employerVacancyId + "' class='fas fa-trash-alt btn-delete'></i>"
                        ]).draw(false);
                        console.log(result);

                        alert("Job vacancy: " + result.vacancy.jobTitle + " has been created successfuly");
                        $("#create-vacancy-modal").modal("hide");
                        ClearJobCreateFields();
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        }
    });

    $("#btn-update-vacancy").click(function () {
         
         $("#update-job-selector option:selected").val();
         $("#update-locations-selector option:selected");
         
         $("#update-end-date").val();
    });

    $("#btn-close-updatevacancy").click(function () {

    });

    $("body").on("click", ".btn-delete", function () {
        var jobId = $(this).attr("data-vid");
        var tr = $(this).parent().parent();
        var table = $("#vacancy-listing").DataTable();
        table.row(tr).remove().draw();
       
    });

    $("body").on("click", ".btn-edit", function () {
        var jobId = $(this).attr("data-vid");
        var tr = $(this).parent().parent();
        var table = $("#vacancy-listing").DataTable();
        var rowData = table.row(tr).data();

        rowData[0] = "Job title";
        rowData[0] = "Job Type";
        rowData[0] = "Locations";
        rowData[0] = "Start Date";
        rowData[0] = "End Date";

        table.row(tr).data(rowData).invalidate();
        console.log(jobId);
        RestoreVacancyOptions(jobId);
        $("#edit-vacancy-modal").modal("show");
    });

    function RestoreVacancyOptions(jobId)
    {
        $.ajax({
            url: "/employer/restore-joblocs?jobId=" + jobId,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (result) {
                console.log(result);
                var arrayValues = [];
                $.each(result.jobLocations, function (i, value) {
                    arrayValues.push(value.locationId);
                });

                $("#update-locations-selector").val(arrayValues);
                $("#update-job-selector").val(result.jobType.jobId);
                $("#update-vacancy-job-post-name").val(result.vacancy.jobTitle);
                $("#update-start-date").val(result.vacancy.html_startDate);
                $("#update-end-date").val(result.vacancy.html_endDate);
            },
            error: function (result) {
                console.log(result);
            }
        });
    }

    function ClearJobCreateFields()
    {
        $("#locations-selector option:selected").removeAttr("selected");
        $("#vacancy-job-post-name").val("");
        $("#job-type-selector option:selected").removeAttr("selected");
        $("#end-date").val("");
    }
});