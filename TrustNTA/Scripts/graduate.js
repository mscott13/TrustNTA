$(document).ready(function () {
    var originalCreatedJobsData;
    var currentEditRecord;

    var jobListingTable = $("#seeker-jobs-listing").DataTable({
        "pageLength": 50,
        "columns": [
            null,
            null,
            { "width": "23%" },
            null,
            null,
            null,
            null,
        ]
    });
    originalCreatedJobsData = $("#seeker-jobs-listing").DataTable().data().toArray();

    $("body").on("click", ".btn-edit", function () {
        $("#update-job-modal").modal("show");
        currentEditRecord = $(this);
        RestoreJobData($(currentEditRecord).attr("data-jobId"));
    })


    $(".jpost-filter").click(function () {
        $("#search-filter-modal").modal("show");
    });

    $("#btn-filter-search").click(function () {
        var jobTitle = $("#filter_jobTitle").val();
        var company = $("#filter_company option:selected").val();
        var jobType = $("#filter_jobType option:selected").val();
        var location = $("#filter_location option:selected").val();
        var vacancyStatus = $("#filter_vacancyStatus").val();
        var startDate = $("#filter_startDate").val();
        var endDate = $("#filter_endDate").val();
    });

    $("#btn-show-create-modal").click(function () {
        $("#create-job-modal").modal("show");
    });

    $("#uploadFileInput").change(function () {
        if ($(this)[0].files.length > 0)
        {
            $("#fileNamesLabel").text($("#uploadFileInput")[0].files[0].name);
        }
    });

    $("#update-uploadFileInput").change(function () {
        if ($(this)[0].files.length > 0) {
            $("#update-fileNamesLabel").text($("#update-uploadFileInput")[0].files[0].name);
        }
    });

    $("#btn-update-post").click(function () {
        var jobType = $("#update-job-type-selector option:selected").val();
        var locations = $("#update-locations-selector option:selected");
        var uploadFileInput = $("#update-uploadFileInput");
        var status = $("#update-job-status-selector option:selected").val();

        var jobLocations = [];
        $(locations).each(function () {
            var location = {};
            location.locationId = $(this).val();
            location.locationName = $(this).text();
            jobLocations.push(location);
        });

        var fileName = "";
        if ($("#update-uploadFileInput")[0].files.length > 0) {
            var data = new FormData();
            $.each($("#update-uploadFileInput")[0].files, function (i, file) {
                data.append("file", file)
            });
            data.append("test", "information");

            $.ajax({
                url: "/graduate/uploadresume",
                type: "POST",
                cache: false,
                data: data,
                contentType: false,
                processData: false,
                dataType: "json",
                async: false,
                success: function (result) {
                    console.log(result)
                    fileName = result.fileName;
                },
                error: function (result) {
                    console.log(result)
                }
            });
        }

        var json = JSON.stringify({ "jobType": jobType, "isSeeking": status, "resume": fileName, locations: jobLocations, "seekerJobId": currentEditRecord.attr("data-jobId") });
        $.ajax({
            url: "/graduate/UpdateSeekerJob",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: json,
            success: function (result) {
                console.log(result);

                if (result.status === "updated") {
                    $("#create-job-modal").modal("hide");

                    var tr = $(currentEditRecord).parent().parent();
                    var table = $("#seeker-jobs-listing").DataTable();
                    var rowData = table.row(tr).data();

                    rowData[0] = result.updatedJob.formattedDate;
                    rowData[1] = result.updatedJob.jobTypeName;
                    rowData[2] = result.updatedJob.formattedLocations;

                    if (result.updatedJob.resume !== null) {
                        rowData[3] = "<a href='/uploads/'" + result.updatedJob.resume+">" + result.updatedJob.resume + "</a>";
                    }

                    rowData[4] = result.updatedJob.jobStatusDesc;
                    rowData[5] = '<i class="fas fa-edit btn-edit" data-jobId="' + result.updatedJob.seekerJobId + '"></i>';
                    rowData[6] = '<i class="fas fa-trash-alt btn-delete" data-jobId="' + result.updatedJob.seekerJobId + '"></i>'
                    table.row(tr).data(rowData).invalidate();
                    $("#update-job-modal").modal("hide");

                    setTimeout(function () {
                        $(tr).addClass("row-highlight").delay(5000).queue(function (next) {
                            $(this).removeClass("row-highlight");
                            next();
                        });
                    }, 200);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    });

    $("body").on("click", ".btn-delete", function () {
        var jobId = $(this).attr("data-jobId");
        var json = JSON.stringify({ "jobId": jobId });
        var row = $(this).parent().parent();

        $.ajax({
            url: "/graduate/deleteseekerjob",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: json,
            success: function (result) {
                if (result.status === "updated")
                {
                    console.log(result);
                    var table = $("#seeker-jobs-listing").DataTable();
                    table.row(row).remove().draw();
                }
            },
            error: function (result) {
                console.log(result);
            }
        })
    });

    $("#btn-post-job").click(function () {
        var jobType = $("#job-type-selector option:selected").val();
        var locations = $("#locations-selector option:selected");
        var uploadFileInput = $("#uploadFileInput");
        var status = $("#job-status-selector option:selected").val();

        var jobLocations = [];
        $(locations).each(function () {
            var location = {};
            location.locationId = $(this).val();
            location.locationName = $(this).text();
            jobLocations.push(location);
        });

        var fileName = "";
        if ($("#uploadFileInput")[0].files.length > 0)
        {
            var data = new FormData();
            $.each($("#uploadFileInput")[0].files, function (i, file) {
                data.append("file", file)
            });
            data.append("test", "information");

            $.ajax({
                url: "/graduate/uploadresume",
                type: "POST",
                cache: false,
                data: data,
                contentType: false,
                processData: false,
                dataType: "json",
                async: false,
                success: function (result) {
                    console.log(result)
                    fileName = result.fileName;
                },
                error: function (result) {
                    console.log(result)
                }
            });
        }

        var json = JSON.stringify({ "jobType": jobType, "isSeeking": status, "resume": fileName, locations: jobLocations });
        $.ajax({
            url: "/graduate/createseekerjob",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: json,
            success: function (result) {
                console.log(result);

                if (result.status === "created")
                {
                    $("#create-job-modal").modal("hide");
                    var row = jobListingTable.row.add([
                        result.createdJob.formattedDate,
                        result.createdJob.jobTypeName,
                        result.createdJob.formattedLocations,
                        '<a href="/uploads/' + result.createdJob.resume + '"><i style="margin-right: 5px;" class="fas fa-file-download"></i>' + result.createdJob.resume + '</a>',
                        result.createdJob.jobStatusDesc,
                        '<i class="fas fa-edit btn-edit" data-jobId="' + result.createdJob.seekerJobId + '">',
                        '<i class="fas fa-trash-alt btn-delete" data-jobId="' + result.createdJob.seekerJobId + '"></i>'
                    ]).draw(false).node();

                    setTimeout(function () {
                        $(row).addClass("row-highlight").delay(5000).queue(function (next) {
                            $(this).removeClass("row-highlight");
                            next();
                        });
                    }, 200);
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    });

    $("body").on("click", ".btn-delete", function () {
        var jobId = $(this).attr("data-vid");
        var json = JSON.stringify({ "jobId": jobId });
        var row = $(this);

        $.ajax({
            url: "/employer/delete-vacancy",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: json,
            success: function (result) {
                if (result.status === "job_deleted") {
                    var tr = $(row).parent().parent();
                    var table = $("#vacancy-listing").DataTable();
                    table.row(tr).remove().draw();
                }
            },
            error: function (result) {
                console.log(result);
            }
        });
    });

    $("body").on("click", ".remove-location-avail", function () {

    });

    $("body").on("click", ".remove-jobtype-pref", function () {

    });

    $("#btn-add-loc-avail").click(function () {
        var value = $("#loc-prefs-selector option:selected").val();
        var text = $("#loc-prefs-selector option:selected").text();
        var target = $("#loc-prefs-container");
    });

    $("#btn-add-job-prefs").click(function () {
        var value = $("#job-prefs-selector option:selected").val();
        var text = $("#job-prefs-selector option:selected").text();
        var target = $("#jtypes-container");
    });

    arguments
    $("#")



    function RestoreJobData(jobId)
    {
        $.ajax({
            url: "/graduate/restore-joblocs?jobId=" + encodeURIComponent(jobId),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (result) {
                if (result.status === "retrieved")
                {
                    console.log(result);

                    var arrayValues = [];
                    $.each(result.job.locations, function (i, value) {
                        arrayValues.push(value.locationId);
                    });

                    $("#update-locations-selector").val(arrayValues);
                    $("#update-job-type-selector").val(result.job.jobType);
                    $("#update-fileNamesLabel").text(result.job.resume);

                    var isSeeking = result.job.isSeeking;
                    console.log(isSeeking.toString());
                    $("#update-job-status-selector").val(isSeeking.toString())
                }
            },
            error: function (result) {
                console.log(result);
            }
        })
    }
});