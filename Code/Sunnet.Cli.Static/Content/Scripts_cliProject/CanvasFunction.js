
var defaultWidth = 120;
var defaultHeight = 120;
var minPadding = 1;//当设置layout的尺寸超过canvas时，默认间距
var originTarget; //上次选中的object
var originoCoords; //上次选中object的oCoords值
var selectable = true;
var isGroup = false; //是否选中组

//新增背景图片时的对象模板
var addedBackgroundImage = JSON.parse('{"type": "image", "originX": "left", "originY": "top", "left": 0, "top": 0, "width": 0, "height": 0, "fill": "rgb(0,0,0)", "stroke": null, "strokeWidth": 1, "strokeDashArray": null, "strokeLineCap": "butt", "strokeLineJoin": "miter", "strokeMiterLimit": 10, "scaleX": 1, "scaleY": 1, "angle": 0, "flipX": false, "flipY": false, "opacity": 1, "shadow": null, "visible": true, "clipTo": null, "backgroundColor": "", "fillRule": "nonzero", "globalCompositeOperation": "source-over", "transformMatrix": null, "src": "", "filters": [], "crossOrigin": "", "alignX": "none", "alignY": "none", "meetOrSlice": "meet"}');


//initialize Canvas
function InitCanvas(element, backgroundColor)
{
    var canElement = "content-body";
    if (element)
        canElement = element;
    if (!backgroundColor)
        backgroundColor = "";



    canvas = new fabric.Canvas(canvasElement, {
        backgroundColor: backgroundColor,
        width: $("#" + canElement + "").innerWidth() - 20,
        height: ($("#" + canElement + "").innerWidth() - 20) * (565 / 1024)
    });


    $("#spanCanvasWidth").html(canvas.width.toFixed(2) + "px");
    $("#spanCanvasHeight").html(canvas.height.toFixed(2) + "px");

    canvas.on("object:selected", function (obj)
    {  //when object selected 
        if (obj.target.type == "image")
        {
            if (window.ShowSize)
            {
                ShowSize(obj);
            }
            originTarget = JSON.parse(JSON.stringify(obj));
            originoCoords = obj.target.oCoords;
            if (canvas.getActiveGroup() != null)
                isGroup = true;
            else
                isGroup = false;
        }
        else
        {
            isGroup = true;
            if (window.CloseSize)
            {
                CloseSize();
            }
        }
    });

    canvas.on("selection:cleared", function (obj)
    {  //when object lost selection
        
        if (!isGroup)
        {
            setTimeout(function ()
            {
                if (window.ResizeObjSize)
                {
                    ResizeObjSize();
                }
                if (window.CloseSize)
                {
                    CloseSize();
                }
            }, 20);
        }
    });

    canvas.on("object:moving", function (obj)
    {  //when object is moving
        if (obj.target.type == "image")
        {
            if (window.ShowSize)
            {
                ShowSize(obj);
            }
        }
    });

    canvas.on("object:scaling", function (obj)
    {  //when object is scaling
        if (obj.target.type == "image")
        {
            if (window.CloseSize)
            {
                CloseSize();
            }
        }
    });

    canvas.on('object:modified', function (obj)
    {  //when object changed(moved or resized)    
        if (obj.target.type == "text")
        {  //instruction text移动位置
            if (obj.target.left < 0)
                obj.target.left = 1;
            if (obj.target.left + obj.target.width > canvas.width)
                obj.target.left = canvas.width - obj.target.width;
            if (obj.target.top < 0)
                obj.target.top = 1;
            if (obj.target.top + obj.target.height > canvas.height)
                obj.target.top = canvas.height - obj.target.height;
            obj.target.setCoords();//重新计算坐标 
        }
        if (obj.target.type == "image")
        {
            if (window.RePositionLayout)
            {
                RePositionLayout(obj);
            }
        }
    });
}

//重置图片的位置，确保不超出画布范围
function RePositionLayout(obj)
{
    if (obj.target) {
        //var beforeLeft = obj.target.left;
        //var beforeTop = obj.target.top;
        //obj.target.left = obj.target.left - (obj.target.width / 2.0);
        //obj.target.top = obj.target.top - (obj.target.height / 2.0);
         
        var realWidth = obj.target.scaleX * obj.target.width;
        var realHeight = obj.target.scaleY * obj.target.height;

        if ((realHeight + obj.target.top > canvas.height)
            || (realWidth + obj.target.left > canvas.width)
            || (obj.target.left < 0) || (obj.target.top < 0))
        {

            //高度或宽度超出范围时，等比缩放

            var rate = realHeight / realWidth; //图片比例
            if (realWidth > canvas.width)
            {
                obj.target.scaleX = (canvas.width - minPadding) / obj.target.width;
                obj.target.scaleY = (canvas.width - minPadding) * rate / obj.target.height;
            }
            if (realHeight > canvas.height)
            {
                obj.target.scaleY = (canvas.height - minPadding) / obj.target.height;
                obj.target.scaleX = ((canvas.height - minPadding) / rate) / obj.target.width;
            }

            if (obj.target.top.toFixed(2) < originTarget.target.top.toFixed(2)) //向上拖动或拉伸，top+
            {
                if (obj.target.scaleY.toFixed(2) == originTarget.target.scaleY.toFixed(2)) //大小不变时，返回之前位置
                    obj.target.top = originTarget.target.top;
                else
                {
                    if (obj.target.top + (obj.target.height * obj.target.scaleY) > canvas.height || obj.target.top < 0)
                        obj.target.top = 1;
                }

            }
            else  //向下拖动或拉伸，top-
            {
                var newTop = originTarget.target.top - ((obj.target.scaleY - originTarget.target.scaleY) * originTarget.target.height);
                obj.target.top = newTop > 0 ? newTop : 1;
            }


            if (obj.target.left.toFixed(2) < originTarget.target.left.toFixed(2)) //向左拖动或拉伸
            {
                if (obj.target.scaleX.toFixed(2) == originTarget.target.scaleX.toFixed(2)) //大小不变时，返回之前位置
                    obj.target.left = originTarget.target.left;
                else
                {
                    if (obj.target.left + (obj.target.width * obj.target.scaleX) > canvas.width || obj.target.left < 0)
                        obj.target.left = 1;
                }

            }
            else  //向右拖动或拉伸
            {
                var newLeft = originTarget.target.left - ((obj.target.scaleX - originTarget.target.scaleX) * originTarget.target.width);
                obj.target.left = newLeft > 0 ? newLeft : 1;
            }
            //只改变位置，没改变大小时，坐标为之前的设置。否则为新的
            if (obj.target.scaleX == originTarget.target.scaleX && obj.target.scaleY == originTarget.target.scaleY)
            {
                obj.target.oCoords = originoCoords;
            }
            else
            {
                obj.target.setCoords();//重新计算坐标
            }
            canvas.renderAll();
            if (window.ShowSize)
            {
                ShowSize(obj);
            }
            originTarget = JSON.parse(JSON.stringify(obj));
        }
        else
        {
            originTarget = JSON.parse(JSON.stringify(obj));
            originoCoords = obj.target.oCoords;
        }
        $("#txtCanvaWidth").val((obj.target.scaleX * obj.target.width / canvas.width *100.00).toFixed(2));
        $("#txtCanvaHeight").val((obj.target.scaleY * obj.target.height / canvas.height * 100.00).toFixed(2));

        $("#hidCanvaWidth").val((obj.target.scaleX * obj.target.width).toFixed(2));
        $("#hidCanvaHeight").val((obj.target.scaleY * obj.target.height).toFixed(2));
    }
}


//Initialize the image loaded
function InitPicture(layout, answers, customBackgroundImage)
{

    canvas.clear();
    InitLines();
    var curScreenWidth = $("div.canvas-container").innerWidth();
    var curScreenHeight = $("div.canvas-container").innerHeight();
    var existLayouts = []; //The existing layout
    var models;

    if (layout)
    {
        models = JSON.parse(layout);
        if (models && models.objects && models.objects.length)
        {
            existLayouts = models.objects;
        }
    }

    //若canvas已存在背景色，则不覆盖
    if (customBackgroundImage)
    {
        delete canvas["backgroundColor"];
        canvas.setBackgroundImage(customBackgroundImage, canvas.renderAll.bind(canvas), {
            width: canvas.width,
            height: canvas.height,
            originX: 'left',
            originY: 'top'
        });
    }
    else
    {
        if (models && models.backgroundImage && models.backgroundImage.src && !(canvas.backgroundColor))
        {
            delete canvas["backgroundColor"];
            canvas.setBackgroundImage(models.backgroundImage.src, canvas.renderAll.bind(canvas), {
                width: canvas.width,
                height: canvas.height,
                originX: 'left',
                originY: 'top'
            });
        }
    }

    var _rowIndex = 0;
    var _cellIndex = 0;
    var _lineIndex = 0;

    if (answers && answers.length > 0)
    {
        answers.sort(function (a, b)
        {
            return a.ID - b.ID;
        });  //order by ID

        for (var i = 0; i < answers.length; i++)
        {
            var answer = answers[i];
            var tmpPicture = "";
            if (answer.Picture)
                tmpPicture = jsonModel.BasePath + "/" + answer.Picture + "?id=" + answer.ID + "&sort=" + i;
            if (answer.TargetImage)
                tmpPicture = jsonModel.BasePath + "/" + answer.TargetImage + "?id=" + answer.ID + "&sort=" + i;
            if (!tmpPicture) //如果没有上传图片，则使用固定图片填充
                tmpPicture = getUploadUrl() + "/" + 'content/images/layoutPlaceholder.png?id=' + answer.ID + "&sort=" + i;

            if (tmpPicture)
            {
                fabric.Image.fromURL(tmpPicture, function (img)
                {
                    LoadImg();
                    function LoadImg()
                    {
                        if (img._element)
                        {
                            var answerId = img._element.src.substring(img._element.src.indexOf('?id=') + 4, img._element.src.indexOf('&sort'));  //current answerid
                            if (answerId == answers[_lineIndex].ID)
                            {  //按answer的顺序加载
                                _lineIndex++;
                                var sort = Number(img._element.src.substring(img._element.src.indexOf('&sort=') + 6));
                                //替换图片当成不存在处理（重新计算比例）
                                var preLayouts = existLayouts.filter(function (item) { return item.id == answerId && item.src.indexOf("layoutPlaceholder.png") < 0 });
                                if (preLayouts.length > 0)
                                {   //If the template already exists
                                    //初始化已存在的占位符
                                    ExistedInit(preLayouts, curScreenWidth, img, false, sort);
                                }
                                else
                                {   //将图片按比例缩放
                                    var widthRate = defaultWidth / img.width;
                                    var heightRate = defaultHeight / img.height;
                                    var rate = 1;
                                    if (widthRate < 1 || heightRate < 1)
                                    {
                                        rate = widthRate < heightRate ? widthRate : heightRate;
                                    }  //Calculating zoom in or out
                                    var newLeft = 30 + (defaultWidth + 10) * _cellIndex;
                                    var newTop = 30 + (defaultHeight + 10) * _rowIndex;
                                    var imageList = canvas._objects.filter(function (obj)
                                    {
                                        return obj.type == "image"
                                    });
                                    img.set({
                                        height: img.height * rate,
                                        width: img.width * rate,
                                        sort: sort,
                                        selectable: selectable
                                    });

                                    AddedInit(imageList, img, newLeft, newTop, answerId);

                                    if (img.left + img.width > curScreenWidth)  //If it is greater than the width of the current line,then next line
                                    {
                                        _cellIndex = 0;  //first _cellIndex
                                        _rowIndex++;   //new row
                                        img.set({
                                            left: 30 + (defaultWidth + 10) * _cellIndex,
                                            top: 30 + (defaultHeight + 10) * _rowIndex
                                        });
                                    }

                                    _cellIndex++;
                                }
                                img.active = false;
                                canvas.add(img).setActiveObject(img);

                                //设置完最后一张图片后，添加InstructionText，以确保不被图片遮盖
                                if (_lineIndex == answers.length)
                                {
                                    if ($.isFunction(window.addInstructionText))
                                    {
                                        var instruction = existLayouts.filter(function (obj)
                                        {
                                            return obj.type == "text"
                                        })[0];
                                        if (instruction)
                                        {
                                            if ((preScreenWidth != curScreenWidth && preScreenWidth > 0))
                                                //If the screen before and now is not the same as the screen size, recalculate the left margin
                                            {
                                                //If on the left, then calculate the left margin, opposite to calculate the right margin
                                                if (instruction.left < (preScreenWidth / 2))
                                                    instruction.left = (instruction.left / preScreenWidth) * curScreenWidth;
                                                else
                                                    instruction.left = curScreenWidth -
                                        ((preScreenWidth - instruction.left - instruction.width) / preScreenWidth) * curScreenWidth - instruction.width;
                                            }
                                            canvas.InstructionLeft = instruction.left;
                                            canvas.InstructionTop = instruction.top;
                                        }
                                        addInstructionText(canvas);
                                    }
                                }
                            }
                            else
                            {
                                setTimeout(LoadImg, 100);
                            }
                        }
                    }
                });
            }
        }
    }
}

//Initialize the image loaded
function InitPicture_Layout(layout, noInitPosition)
{
    InitLines();
    var curScreenWidth = $("div.canvas-container").innerWidth();
    var existLayouts = []; //The existing layout    
    if (layout)
    {
        var models = JSON.parse(layout);
        if (models.backgroundImage && models.backgroundImage.src)
        {
            canvas.setBackgroundImage(models.backgroundImage.src, canvas.renderAll.bind(canvas), {
                width: canvas.width,
                height: canvas.height,
                originX: 'left',
                originY: 'top'
            });
        }
        if (models && models.objects && models.objects.length)
        {
            existLayouts = models.objects;
        }
    }

    var _rowIndex = 0;
    var _cellIndex = 0;
    var _imgIndex = 1;  // In order to load images
    for (var i = 1; i <= Number($("#NumberOfImages").val()) ; i++)
    {
        fabric.Image.fromURL("/Content/images/layout_" + i + ".jpg?id=" + i, function (img)
        {
            var answerId = img._element.src.substring(img._element.src.indexOf('?id=') + 4);  //current answerid
            LoadImg();

            function LoadImg()
            {
                if (answerId == _imgIndex)
                {
                    var preLayouts = existLayouts.filter(function (item) { return item.id == answerId });
                    if (preLayouts.length > 0)
                    {   //If the template already exists
                        //初始化已存在的占位符
                        ExistedInit(preLayouts, curScreenWidth, img, noInitPosition, 0);
                    }
                    else
                    {   //Initialize the image loaded

                        var overlap = false;
                        var newLeft = 30 + (defaultWidth + 10) * _cellIndex;
                        var newTop = 30 + (defaultHeight + 10) * _rowIndex;
                        var imageList = canvas._objects.filter(function (obj) { return obj.type == "image" });

                        //初始化新增的占位符
                        AddedInit(imageList, img, newLeft, newTop, answerId);

                        if (img.left + img.width > curScreenWidth)  //If it is greater than the width of the current line,then next line
                        {
                            _cellIndex = 0;  //first _cellIndex
                            _rowIndex++;   //new row
                            img.set({
                                left: 30 + (defaultWidth + 10) * _cellIndex,
                                top: 30 + (defaultHeight + 10) * _rowIndex
                            });
                        }

                        _cellIndex++;
                    }
                    img.active = false;
                    canvas.add(img).setActiveObject(img);

                    _imgIndex++;
                }
                else
                {
                    setTimeout(LoadImg, 100);
                }
            }
        });
    }
}
//end Initialize the image loaded

//初始化已存在的占位符
function ExistedInit(preLayouts, curScreenWidth, img, noInitPosition, sort)
{
    var preLayout = preLayouts[0];
    var preWidth = preLayout.width * preLayout.scaleX;

    var width = preLayout.width * preLayout.scaleX; //model.realwidth
    var height = preLayout.height * preLayout.scaleY; //model.realheight

    if ((preScreenWidth != curScreenWidth && preScreenWidth > 0) && !noInitPosition)
        //If the screen before and now is not the same as the screen size, recalculate the left margin
    {
        width = preLayout.width * preLayout.scaleX * (curScreenWidth / preScreenWidth); //model.realwidth
        var preHeight = preLayout.height * preLayout.scaleY; //model.realheight
        height = (width / preWidth * preHeight);
        var preScreenHeight = preScreenWidth * (565 / 1024);
        var curScreenHeight = curScreenWidth * (565 / 1024);

        if (preLayout.left < (preScreenWidth / 2))
        {
            preLayout.left = (preLayout.left / preScreenWidth) * curScreenWidth;
        }
        else
            preLayout.left = curScreenWidth - ((preScreenWidth - preLayout.left - preWidth) / preScreenWidth) * curScreenWidth - width;

        if (preLayout.top < (preScreenHeight / 2))
            preLayout.top = (preLayout.top / preScreenHeight) * curScreenHeight;
        else
            preLayout.top = curScreenHeight - ((preScreenHeight - preLayout.top - preHeight) / preScreenHeight) * curScreenHeight - height;
    }

    //当替换前的图片和替换后的图片比例不同时，进行比例转换
    var widthRate = width / (img.width * img.scaleX);
    var heightRate = height / (img.height * img.scaleY);
    rate = widthRate < heightRate ? widthRate : heightRate;

    img.set({     //Attribute set before
        left: preLayout.left, //靠左距离
        top: preLayout.top, //靠上距离
        height: preLayout.height, //高度
        width: preLayout.width, //宽度
        scaleX: preLayout.scaleX, //横向拉伸倍数
        scaleY: preLayout.scaleY, //竖向拉伸倍数
        id: preLayout.id,
        sort: sort,
        angle: preLayout.angle,
        active: false,
        lockUniScaling: true, //锁定反方向大小
        hasRotatingPoint: false, //去除旋转框
        selectable: selectable,  //是否可选择
        width: img.width * img.scaleX * rate,
        height: img.height * img.scaleY * rate,
        scaleX: 1,
        scaleY: 1
    });
}

//初始化新增的占位符
function AddedInit(imageList, img, newLeft, newTop, answerId)
{
    for (var i = 0; i < imageList.length; i++)
    {
        if (newLeft == imageList[i].left)
        {
            if (newTop == imageList[i].top)
            {//新图覆盖了旧图; 新图错开位置显示
                newLeft = imageList[i].left + 20;
                newTop = imageList[i].top + 20;
            }
        }
    }

    img.set({
        left: newLeft,
        top: newTop,
        id: answerId,
        active: false,
        lockUniScaling: true,
        hasRotatingPoint: false
    });
}

function InitLines()
{

    var w = $("div.canvas-container").innerWidth();
    var h = ($("div.canvas-container").innerWidth()) * (565 / 1024);

    for (var x = 1; x < w; x += 50)
    {
        canvas.insertAt(new fabric.Line([x, 0, x, h], { stroke: "#C0C0C0", strokeWidth: 0.5, selectable: false, strokeDashArray: [1, 1] }), 0);
    };

    for (var y = 1; y < h; y += 50)
    {
        canvas.insertAt(new fabric.Line([0, y, w, y], { stroke: "#C0C0C0", strokeWidth: 0.5, selectable: false, strokeDashArray: [1, 1] }), 0);
    };
}

//The page size changes triggered
function CanvasResize(obj, preWidth, canvasElement) {
   
    var models = JSON.parse(obj.replace("\\", ""));
    if (models && models.objects.length)
    {
        var tmpLength = models.objects.length;

        for (var i = tmpLength - 1; i >= 0; i--)
            if (models.objects[i].type == "line")
                models.objects.splice(i, 1);

        canvas.clear();
        $("div[id='content-body'] div.canvas-container").remove();
        $("div[id='previewLayout'] div.canvas-container").remove(); // remove preview page
        closeModal("#modalLayoutPreview");
        $("#content-body").append("<canvas id='cav_layout'>(Your browser doesn't support canvas)</canvas>");

        InitCanvas(canvasElement);

        var curScreenWidth = $("div.canvas-container").innerWidth();
        if (curScreenWidth != preWidth)
        {
            InitPosition(models, curScreenWidth, preWidth);
            if (models.backgroundImage && models.backgroundImage.width)
            {
                models.backgroundImage.width = curScreenWidth;
            }
        }

        delete canvas["backgroundImage"];
        models.objects.sort(function (a, b)
        {
            return a.sort - b.sort;
        });  //order by sort
        canvas.loadFromDatalessJSON(JSON.stringify(models), function() { canvas.renderAll.bind(canvas); });

        setTimeout(function ()
        {
            var w = $("div.canvas-container").innerWidth();
            var h = ($("div.canvas-container").innerWidth()) * (1024 / 565);

            for (var x = 1; x < w; x += 50)
            {
                canvas.insertAt(new fabric.Line([x, 0, x, h], {
                    stroke: "#C0C0C0", strokeWidth: 0.5, selectable: false, strokeDashArray: [1, 1]
                }), 0);
            };

            for (var y = 1; y < h; y += 50)
            {
                canvas.insertAt(new fabric.Line([0, y, w, y], {
                    stroke: "#C0C0C0", strokeWidth: 0.5, selectable: false, strokeDashArray: [1, 1]
                }), 0);
            };
        }, 100);
    }
}

function setCustomCanvas()
{
    var _obj = canvas._objects;
    for (var i = 0; i < _obj.length; i++)
    {
        if (_obj[i].type == "image")
            _obj[i].selectable = true;
    }
    canvas.renderAll();
}

//To recalculate the image position
function InitPosition(models, curScreenWidth, preWidth)
{

    if (models.objects && models.objects.length > 0)
    {
        for (var i = 0; i < models.objects.length; i++)
        {
            var model = models.objects[i];
            var preScreenHeight = preWidth * (565 / 1024);
             var curScreenHeight = curScreenWidth * (565 / 1024);
         
            // * (curScreenWidth / preWidth * 1.00);
             model.scaleX = model.scaleX * (curScreenWidth / preWidth * 1.00);
             model.scaleY = model.scaleY * (curScreenHeight / preScreenHeight * 1.00);

             var width = model.width * model.scaleX; //model.realwidth
             var height = model.height * model.scaleY; //model.realwidth
             model.left = (model.left / preWidth) * curScreenWidth;
            //if (model.left < (preWidth / 2))
            //{
            //    model.left = (model.left / preWidth) * curScreenWidth;
            //}
            //else
            //{
            //    model.left = curScreenWidth - ((preWidth - model.left - width) / preWidth) * curScreenWidth - width;
            //}
            //if (model.top < (preScreenHeight / 2))
            //{
            //    model.top = (model.top / preScreenHeight) * curScreenHeight;
            //}
            //else
            //{
            //    model.top = curScreenHeight - ((preScreenHeight - model.top - height) / preScreenHeight) * curScreenHeight - height;
            //}
        
             model.top = (model.top / preScreenHeight) * curScreenHeight;
            if (selectedObj != null) {
                if (selectedObj.target.id == model.id)
                {
                    selectedObj.target.scaleX = model.scaleX;
                    selectedObj.target.scaleY = model.scaleY;
                    selectedObj.target.left = model.left;
                    selectedObj.target.top = model.top;

                    $("#hidCanvaHeight").val(height);
                    $("#hidCanvaWidth").val(width);
                    $("#hidCanvaLeft").val(model.left - width/2.00);
                    $("#hidCanvaTop").val(model.top - height/2.00);
                    
                    $("#txtCanvaHeight").val((height / curScreenHeight*100.00).toFixed(2));
                    $("#txtCanvaWidth").val((width / curScreenWidth * 100.00).toFixed(2));
                    $("#txtCanvaLeft").val(((model.left + width / 2.00) / curScreenWidth * 100.00).toFixed(2));
                    $("#txtCanvaTop").val(((model.top + height / 2.00) / curScreenHeight * 100.00).toFixed(2));

                    $("#spanCanvasWidth").html(curScreenWidth.toFixed(2) + "px");
                    $("#spanCanvasHeight").html(curScreenHeight.toFixed(2) + "px");
                }
            }
          
        }
    }
}

function GetLayouts(layoutElement, backgroundImgSrc)
{
    if (canvas._objects.length > 0)
    {

        canvas._objects = canvas._objects.filter(function (obj)
        {
            return (obj.type == "image" || obj.type == "text")
        });
        if ($(layoutElement).length > 0)
        {
            $(layoutElement).val(JSON.stringify(canvas.toDatalessJSON(new Array("id", "sort", "lockUniScaling", "hasRotatingPoint", "selectable"))));
        }
        //preview之后还要把网格加上
        InitLines();

        var canvElement = $("div.canvas-container");
        if (canvElement.length > 0 && canvElement.innerWidth() > 0)
        {
            $("#ScreenWidth").val(canvElement.innerWidth());
            $("#ScreenHeight").val(canvElement.innerHeight());
        }
    }

    var preLayout;
    if ($(layoutElement).length > 0 && $(layoutElement).val())
    {
        preLayout = JSON.parse($(layoutElement).val());
        if (canvas.backgroundColor)
        {  //有背景色，直接删除背景图
            delete preLayout["backgroundImage"];
            if (preLayout.background != canvas.backgroundColor)
                preLayout.background = canvas.backgroundColor;  //防止背景色不一样
            $(layoutElement).val(JSON.stringify(preLayout));
        }
        else
        {
            delete preLayout["background"];
            //背景图片改变之后，没有进入到layoutDesign界面就保存时，要更改Layout的值，layout传值null
            if (backgroundImgSrc != null)
            {
                if (!backgroundImgSrc)
                { //没有背景图片，则删除
                    delete preLayout["backgroundImage"];
                    $(layoutElement).val(JSON.stringify(preLayout));
                }
                else
                { //图片被替换                
                    var bgImgSrc = '';
                    if (backgroundImgSrc.indexOf("http:") >= 0)
                        bgImgSrc = backgroundImgSrc;
                    else
                        bgImgSrc = getUploadUrl() + '/upload/' + backgroundImgSrc;

                    if (!preLayout.backgroundImage)
                    {   //如果没有，则新增
                        preLayout.backgroundImage = addedBackgroundImage;
                        preLayout.backgroundImage.src = bgImgSrc;
                        preLayout.backgroundImage.height = canvas.height;
                        preLayout.backgroundImage.width = canvas.width;
                        $(layoutElement).val(JSON.stringify(preLayout));
                    }
                    else
                    {  //如果已存在，则比较
                        if (bgImgSrc != preLayout.backgroundImage.src)
                        {
                            preLayout.backgroundImage.src = bgImgSrc;
                            $(layoutElement).val(JSON.stringify(preLayout));
                        }
                    }
                }
            }
        }
    }

    if ($(layoutElement).val() && $("#CpallsItemLayout").length > 0)
    {
        var jsonLayout = JSON.parse($(layoutElement).val());
        if (jsonLayout && jsonLayout.objects && jsonLayout.objects.length)
        {
            var cpallsItemLayout = '{';
            cpallsItemLayout += '"objects":[';
            for (var i = 0; i < jsonLayout.objects.length; i++)
            {
                var objModel = jsonLayout.objects[i];

                cpallsItemLayout += '{';
                cpallsItemLayout += '"id":' + (objModel.id ? objModel.id : 0) + ',"src":"' + (objModel.src ? objModel.src : "") + '",';
                cpallsItemLayout += '"top":' + objModel.top + ',"left":' + objModel.left + ',';
                cpallsItemLayout += '"height":' + objModel.height * objModel.scaleY + ',"width":' + objModel.width * objModel.scaleX;
                cpallsItemLayout += '},';
            }
            cpallsItemLayout = cpallsItemLayout.substring(0, cpallsItemLayout.length - 1);
            cpallsItemLayout += ']';
            if (jsonLayout.background)
                cpallsItemLayout += ',"background":"' + jsonLayout.background + '"';
            if (jsonLayout.backgroundImage)
                cpallsItemLayout += ',"backgroundImage":{"src":"' + jsonLayout.backgroundImage.src + '"}';
            cpallsItemLayout += '}';

            $("#CpallsItemLayout").val(cpallsItemLayout);
        }
    }
}

function GetLayouts2(layoutElement, backgroundImgSrc) {
    if (canvas._objects.length > 0) {

        canvas._objects = canvas._objects.filter(function (obj) {
            return (obj.type == "image" || obj.type == "text")
        });
        if ($(layoutElement).length > 0) {
            $(layoutElement).val(JSON.stringify(canvas.toDatalessJSON(new Array("id", "sort", "lockUniScaling", "hasRotatingPoint", "selectable"))));
        }
        //preview之后还要把网格加上
        InitLines();

        var canvElement = $("div.canvas-container");
        if (canvElement.length > 0 && canvElement.innerWidth() > 0) {
            $("#ScreenWidth").val(canvElement.innerWidth());
            $("#ScreenHeight").val(canvElement.innerHeight());
        }
    }

    var preLayout;
    if ($(layoutElement).length > 0 && $(layoutElement).val()) {
        preLayout = JSON.parse($(layoutElement).val());
        if (canvas.backgroundColor) {  //有背景色，直接删除背景图
            delete preLayout["backgroundImage"];
            if (preLayout.background != canvas.backgroundColor)
                preLayout.background = canvas.backgroundColor;  //防止背景色不一样
            $(layoutElement).val(JSON.stringify(preLayout));
        }
        else {
            delete preLayout["background"];
            //背景图片改变之后，没有进入到layoutDesign界面就保存时，要更改Layout的值，layout传值null
            if (backgroundImgSrc != null) {
                if (!backgroundImgSrc) { //没有背景图片，则删除
                    delete preLayout["backgroundImage"];
                    $(layoutElement).val(JSON.stringify(preLayout));
                }
                else { //图片被替换                
                    var bgImgSrc = '';
                    if (backgroundImgSrc.indexOf("http:") >= 0)
                        bgImgSrc = backgroundImgSrc;
                    else
                        bgImgSrc = getUploadUrl() + '/upload/' + backgroundImgSrc;

                    if (!preLayout.backgroundImage) {   //如果没有，则新增
                        preLayout.backgroundImage = addedBackgroundImage;
                        preLayout.backgroundImage.src = bgImgSrc;
                        preLayout.backgroundImage.height = canvas.height;
                        preLayout.backgroundImage.width = canvas.width;
                        $(layoutElement).val(JSON.stringify(preLayout));
                    }
                    else {  //如果已存在，则比较
                        if (bgImgSrc != preLayout.backgroundImage.src) {
                            preLayout.backgroundImage.src = bgImgSrc;
                            $(layoutElement).val(JSON.stringify(preLayout));
                        }
                    }
                }
            }
        }
    }

    if ($(layoutElement).val() && $("#CpallsItemLayout").length > 0) {
        var jsonLayout = JSON.parse($(layoutElement).val());
        if (jsonLayout && jsonLayout.objects && jsonLayout.objects.length) {
            var cpallsItemLayout = '{';
            cpallsItemLayout += '"objects":[';
            for (var i = 0; i < jsonLayout.objects.length; i++) {
                var objModel = jsonLayout.objects[i];

                cpallsItemLayout += '{';
                cpallsItemLayout += '"id":' + (objModel.id ? objModel.id : 0) + ',"src":"' + (objModel.src ? objModel.src : "") + '",';
                cpallsItemLayout += '"top":' + (objModel.top + (objModel.height * objModel.scaleY) / 2.00) + ',"left":' + (objModel.left + (objModel.width * objModel.scaleX/22.00)) + ',';
                cpallsItemLayout += '"height":' + objModel.height * objModel.scaleY + ',"width":' + objModel.width * objModel.scaleX;
                cpallsItemLayout += '},';
            }
            cpallsItemLayout = cpallsItemLayout.substring(0, cpallsItemLayout.length - 1);
            cpallsItemLayout += ']';
            if (jsonLayout.background)
                cpallsItemLayout += ',"background":"' + jsonLayout.background + '"';
            if (jsonLayout.backgroundImage)
                cpallsItemLayout += ',"backgroundImage":{"src":"' + jsonLayout.backgroundImage.src + '"}';
            cpallsItemLayout += '}';

            $("#CpallsItemLayout").val(cpallsItemLayout);
        }
    }
}
