(function (window, document, $) {

    /*
    模拟easyui的插件 开发的基于webuploader的上传插件
    依赖easyui的linkbutton、processbar
    */

   // var initTask =  $.Deferred();

    //放这里，不用每次调用jqEasyuiWebuploader函数时都去实例化
    let methods = {
        //getFileIds: function () {
        //    let ary = new Array();
        //    //这里的this是个数组，但是这里只考虑它是单个jq对象的情况
        //    $(this).data('ct').uploader.getFiles('complete').forEach(function (currentValue, index) {
        //        ary.push(currentValue.myData.serverFileId);
        //    });
        //    return ary;
        //},
        checkSaveOrGetFileIds: function () {
            let d = { success: true };

            let up = $(this).data('ct').uploader;

            if (up.getFiles('inited').length > 0)
                d.success = false;
            else if (up.getFiles('queued').length > 0)
                d.success = false;
            else if (up.getFiles('progress').length > 0)
                d.success = false;
            else if (up.getFiles('error').length > 0)
                d.success = false;
            else if (up.getFiles('interrupt').length > 0)
                d.success = false;
            else if (up.getFiles('cancelled').length > 0)
                d.success = false;

            d.data = new Array();
            //这里的this是个数组，但是这里只考虑它是单个jq对象的情况
            up.getFiles('complete').forEach(function (currentValue, index) {
                d.data.push(currentValue.myData.serverFileId);
            });
            return d;
        },
        webUploader: function () {
            return $(this).data('ct').uploader;
        }
       //, initTask: function () {
       //     return initTask;
       // }
    };

    function onClickUploadAll(context) {
        if (context.btnUploadAll.text() == '全部上传') {
            let files = context.uploader.getFiles('cancelled').concat(context.uploader.getFiles('queued'));

            files.forEach(function (curr, index) {
                curr.myData.btnStop.trigger('click');
            });
            context.btnUploadAll.linkbutton('options').text = '全部停止';
        }
        else {
            let files = context.uploader.getFiles('progress').concat(context.uploader.getFiles('queued'));

            files.forEach(function (curr, index) {
                curr.myData.btnStop.trigger('click');
            });

            context.btnUploadAll.linkbutton('options').text = '全部上传';
        }
        $.parser.parse(context.btnUploadAll.parent());
    }
    //全部上传启用禁用控制
    function btnUploadAllED(fileContext) {
        let uploader = fileContext.uploader;
        let btnUploadAll = fileContext.btnUploadAll;
        var files = uploader.getFiles('cancelled');
        var files1 = uploader.getFiles('queued');
        var files2 = uploader.getFiles('progress');
        if (files && files.length > 0 || files1.length > 0 || files2.length > 0)
            btnUploadAll.linkbutton('enable');
        else
            btnUploadAll.linkbutton('disable');
    }

    function onClickFileStop(fileContext) {

        if (fileContext.file.getStatus() == 'deleted')
            return;

        var cls = fileContext.btnStop.linkbutton('options').iconCls;

        if (cls == 'icon-kaishi') {
            fileContext.uploader.upload(fileContext.file);
            fileContext.btnStop.linkbutton('options').iconCls = 'icon-zanting';
        } else {
            fileContext.uploader.cancelFile(fileContext.file);
            fileContext.btnStop.linkbutton('options').iconCls = 'icon-kaishi';
        }
        $.parser.parse(fileContext.btnStop.parent());
    }
    function onClickFileDel(fileContext) {
        fileContext.uploader.removeFile(fileContext.file.id);
        fileContext.file.setStatus('deleted');
        fileContext.tr.hide(function () {
            fileContext.tr.remove();
        });
    }

    //文件整体上传成功后的处理
    function uploadSuccess(ct) {
        if (ct.file.getStatus() == 'deleted')
            return;

        if (!ct.serverFileId)
            ct.file.setStatus('error');
    }
    //每个分片或非分片上传完成时的处理
    function uploadAccept(ct) {
        if (ct.file.getStatus() == 'deleted')
            return;

        //若此请求返回了id字段则表示上传完成
        if (ct.response.id)
            ct.serverFileId = ct.response.id;
    }
    //上传进度处理
    function uploadProgress(ct) {
        let status = ct.file.getStatus();
        if (status == 'deleted')
            return;
        if (status != 'progress')
            return;
        try {
            ct.statusPreocessbar.progressbar('setValue', parseInt(ct.percentage * 100));
        } catch (e) {
            console.log(ct);
            console.log(status);
        }
    }
    //每个分片上传前处理BearerToken
    function uploadBeforeSend(ct) {
        if (ct.file.getStatus() == 'deleted')
            return;

        ct.data.filemd5 = ct.fileMd5;
        ct.data.chunk_size = ct.object.blob.size;

        //其它静态form表单数据通过webuploader的初始化参数中的formData设置
        if (ct.settings.onChunkBeforeUpload)
            ct.settings.onChunkBeforeUpload(ct);
        //ct.headers.Authorization = "Bearer " + abp.auth.accessToken;

        //如果是最后一个分片则表示是用来合并的请求
        if (ct.object.chunks - 1 === ct.object.chunk)
            ct.file.setStatus('hebing', '正在合并...');
    }
    //文件状态变化时处理
    function fileStatusChanged(ct) {
        //console.log('文件' + ct.file.id + '状态改变:' + ct.oldStatus + ' > ' + ct.newStatus);
        btnUploadAllED(ct);

        if (ct.file.getStatus() == 'deleted')
            return;


        ct.statusTD.attr('class', 'file-status-' + ct.newStatus);
        switch (ct.newStatus) {
            case 'inited':
                ct.statusTD.html('初始化');
                ct.btnStop.linkbutton('disable');
                break;
            case 'cancelled':
            case 'queued':
                ct.statusTD.html('等待上传');
                ct.btnStop.show();
                break;
            case 'progress':
                //注意file.myData = fileStatusChangedContext
                ct.statusTD.empty();
                ct.statusPreocessbar = $('<div class="easyui-progressbar" data-options="value:0" style="width1:100%;" />').appendTo(ct.statusTD);
                $.parser.parse(ct.statusTD);
                break;
            case 'complete':
                //ct.input.val(ct.serverFileId);
                ct.statusTD.empty();
                $('<img src="/img/ok_24.png"  />').appendTo(ct.statusTD);
                ct.btnStop.hide();
                break;
            case 'error':
                ct.statusTD.html('上传出错！请重试');
                ct.btnStop.show();
                break;
            case 'interrupt':
                ct.statusTD.html('上传中断！可续传');
                ct.btnStop.show();
                break;
            case 'invalid':
                ct.statusTD.html('校验失败！');
                ct.btnStop.hide();
                break;
            default:
                ct.statusTD.html(ct.file.statusText);
                ct.btnStop.hide();
        }
    }
    //上传前的校验
    function checkUpload(ct) {
        if (ct.file.getStatus() == 'deleted')
            return;

        ct.uploader.md5File(ct.file).progress(function (percentage) {
            ct.md5Span.text('正在计算md5值/' + parseInt(percentage * 100) + '%');
        }).then(function (val) {
            if (ct.file.getStatus() == 'deleted')
                return;

            ct.fileMd5 = val;//在文件对象上保存md5值
            ct.md5Span.text('');

            //var deferred = $.Deferred();//WebUploader.Base.Deferred();
            if (ct.settings.onCheckUpload) {
                ct.file.setStatus('checkUpload', '正在校验...');
                ct.settings.onCheckUpload(ct).done(function (val) {
                    if (ct.file.getStatus() == 'deleted')
                        return;

                    switch (val.state) {
                        case 0://成功，文件不存在
                            //file.setStatus('queued');//默认给这个状态，无法只传一个文件
                            ct.file.setStatus('cancelled');
                            break;
                        case 1://成功，文件已存在
                            ct.serverFileId = val.data.id;
                            ct.file.setStatus('complete');
                            break;
                        default://不满足大小和类型限制\未授权或其它错误
                            //file.setStatus('invalid');//垃圾控件，这种状态又会变成canceled
                            ct.file.setStatus('cannot', '校验失败');
                    }
                });
            } else {
                ct.file.setStatus('cancelled');
            }
        });
    }
    //文件进入队列时的处理
    function fileQueued(ct) {

        //let $this = fileContext.$this;
        let _this = this;
        //var file = fileContext.file;

        //ct.file.myData = file.myData? $.extend(fileContext, file.myData):fileContext;
        //ct.file.myData = $.extend(ct, ct.file.myData);//注意fileContext一定要在前

        let tr = $('<tr id="' + ct.file.id + '">').appendTo(ct.fileContainer);
        ct.tr = tr;

        let td0 = $('<td>').appendTo(tr);
        let icon = $('<img src="/img/' + ct.file.ext + '.png" onerror="this.src=\'/img/file.png\';this.onerror=null" />').appendTo(td0);
        //let tdinput = $('<input type="hidden" name="attachments[]" />').appendTo(td0);
        //ct.input = tdinput;

        let td1 = $('<td>').appendTo(tr);
        let title = $('<h3>' + ct.file.name + '</h3>').appendTo(td1);
        let size = $('<span>' + (ct.file.size / 1024 / 1024).toFixed(2) + 'M</span>').appendTo(td1);//这个只用来显示文件大小，单位m
        let md5 = $('<span />').appendTo(td1);
        ct.md5Span = md5;

        let td2 = $('<td />').appendTo(tr);
        ct.statusTD = td2;

        let tdm = $('<td>').appendTo(tr);
        ct.settings.columnButtons.forEach(function (item,index) {
            if (item.checkVisiable(ct)) {
                let tempA = $('<a />').appendTo(tdm);
                let opt = $.extend({}, item, {
                    onClick: function () {
                        item.onClick.call(this, ct);
                    }
                });
                tempA.linkbutton(opt);
            }
        });



        let td3 = $('<td>').appendTo(tr);
        let btnStop = $('<a class="easyui-linkbutton" data-options="iconCls:\'icon-kaishi\',plain:true" style="display:none;" />').appendTo(td3);
        ct.btnStop = btnStop;


        let td4 = $('<td>').appendTo(tr);
        let btnDel = $('<a class="easyui-linkbutton" data-options="iconCls:\'icon-remove\',plain:true" />').appendTo(td4);
        ct.btnDel = btnDel;

        $.parser.parse(tr);

        setTimeout(function () {

            btnStop.linkbutton('options').onClick = function () {
                onClickFileStop.bind(_this)(ct);
            };

            btnDel.linkbutton('options').onClick = function () {
                onClickFileDel.bind(_this)(ct);
            };

            //内部可能用到easyui控件，因此放setTimeout内部
            ct.file.on('statuschange', function () {
                //因为上面的file引用了fileContext，因此这里只能扩展fileContext，不能创建一个新对象
                let fileStatusChangedContext = $.extend(ct, { oldStatus: arguments[1], newStatus: arguments[0] });
                fileStatusChanged.bind(_this)(fileStatusChangedContext);
            });
            if (!ct.serverFileId)
                checkUpload.bind(_this)(ct);

        }, 0);


    }

    $.fn.jqEasyuiWebuploader = function (options, args) {
        //这里的this是jquery对象，是一个集合
        let _this = this;
        let tp = typeof options;

        if (tp == 'string') {
            return methods[options].call(this, args);
        }
        else if (tp == 'object' || tp == 'undefined') {
            let settings = $.extend({}, $.fn.jqEasyuiWebuploader.defaults, options);

            return this.each(function (index, item) {


                //这里的this等同于item，是DOM对象
                let _this1 = this;
                let $this1 = $(this);

                $this1.addClass('uploader-container');

                settings.dnd = $this1;
                settings.paste = $this1;
                settings.pick = $('<div class="upload-pick">添加</div>').appendTo($this1);
                var uploader = WebUploader.create(settings);
                let btnUploadAll = $('<a class="easyui-linkbutton c1 upload-all-btn" disabled>全部上传</a>').appendTo($this1);
                $.parser.parse($this1);
                let fileContainer = $('<table class="file-container"></table>').appendTo($this1);
                fileContainer.height($this1.height() - btnUploadAll.height() - 10);

                let context = {
                    btnUploadAll: btnUploadAll,
                    fileContainer: fileContainer,
                    uploader: uploader,
                    $this: $this1,
                    settings: settings
                };
                $this1.data('ct', context);

                setTimeout(function () {



                    btnUploadAll.linkbutton('options').onClick = function () {
                        onClickUploadAll.bind(_this)(context);
                    };

                    //uploader.on('beforeFileQueued', function (file) {
                    //    console.log(file);
                    //});

                    uploader.on('fileQueued', function (file) {
                        //虽然会复制conext，但是多个实例引用的实际对象是同一个，更多暂用内存，且调用方访问更方便
                       
                        file.myData = file.myData || {};
                        var ct = $.extend(file.myData, context, { file: file });

                        fileQueued.bind(_this1)(ct);
                    });
                    //每个分片/非分片上传前触发
                    uploader.on('uploadBeforeSend', function (object, data, headers) {
                        var ct = $.extend(object.file.myData, { object: object, data: data, headers: headers });
                        uploadBeforeSend.bind(_this1)(ct);
                    });

                    // 文件上传过程中创建进度条实时显示。
                    uploader.on('uploadProgress', function (file, percentage) {
                        var ct = $.extend(file.myData, { percentage: percentage });
                        uploadProgress.bind(_this1)(ct);
                    });

                    //经过测试
                    //有分片时，每个分片都会执行这里
                    //无分片时也会执行这里
                    uploader.on('uploadAccept', function (object, response) {
                        var ct = $.extend(object.file.myData, { object: object, response: response });
                        uploadAccept.bind(_this1)(ct);
                    });

                    //经过测试，文件整体上传成功才会执行这里，有分片时response代表最后一个分片的请求
                    uploader.on('uploadSuccess', function (file, response) {
                        var ct = $.extend(file.myData, { response: response });
                        uploadSuccess.bind(_this1)(ct);
                    });


                    //initTask.resolve();


                }, 0);

            });
        }
    };

    $.fn.jqEasyuiWebuploader.defaults = {
        //dnd: me//拖拽容器
        //paste: me,//粘贴容器
        prepareNextFile: true,
        chunked: true,
        chunkSize: 1024 * 1024 * 2,
        threads: 10,
        disableGlobalDnd: true,
        //formData: { module: m, permission: permissionName, fileOperation: 0 },//fileOperation其实没必要
        //swf: '/lib/webuploader-0.1.5/Uploader.swf',
        //server: abp.appPath + '/api/attachment/upload',//这里使用默认值其实不太恰当，因为这是一个控件，不应该引入具体业务的概念
        //pick: $picker,
        resize: false,
        onChunkBeforeUpload: undefined,
        onCheckUpload: undefined,
        columnButtons: []
    };

}(window, document, jQuery));