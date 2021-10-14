function insertCase(canvas, n, leftImg, topImg, angle) {

    var images = ['./Output/case1.png', './Output/case2.png', './Output/case3.png', './Output/case4.png'];
    if (n < 1 || n > 4) return;
    var currentId = counter;

    var caseName = `case_${n}_${currentId}`;
    
    fabric.Image.fromURL(images[n - 1], function (img) {
        img.set({
            top: 0,//topImg,
            left: 0, //leftImg,
            originX: 'left',
            originY: 'top',
            hasRotatingPoint: false,
            hasControls: false,
            angle: 0,
        });
        img.perPixelTargetFind = true;

        var text = new fabric.Text(currentId.toString(), {
            fontFamily: 'Arial',
            fontSize: 25
        });
        var bound = img.getBoundingRect();
        text.set('top', (bound.height / 2) - (text.height / 2));
        text.set('left', (bound.width / 2) - (text.width / 2));
        text.rotate(-angle);

        var group = new fabric.Group([img, text], {
            name: caseName,
            left: leftImg,
            top: topImg,
        });
        group.set('angle', angle);
        canvas.add(group);
    });
    counter++;
}
function remove(canvas, activeObject) {
    activeObject.remove();
    reIndex();

    counter--;
}

function insertPallet(canvas, leftImg, topImg)
{
    fabric.Image.fromURL('./Output/pallet.png', function (img) {
        // add background image
        canvas.setBackgroundImage(img, canvas.renderAll.bind(canvas), {
            top: topImg,
            left: leftImg,
            originX: 'left',
            originY: 'top',
            angle: 0,
            hasRotatingPoint: false,
            hasControls: false,
            selectable: false,
        });
    });
}

function reIndex(canvas) {
    var localCounter = 0;
    canvas.forEachObject(function (obj) {
        if (startsWith(obj.name, 'case'))
            obj.name = 'case' + localCounter;
        localCounter++;
    });
}

function startsWith(str, word) {
    return str.lastIndexOf(word, 0) === 0;
}

function intersectingCheck(activeObject) {
    activeObject.setCoords();
    if (typeof activeObject.refreshLast !== 'boolean') {
        activeObject.refreshLast = true
    };

    //loop canvas objects
    activeObject.canvas.forEachObject(function (targ) {
        if (targ === activeObject) return; //bypass self

        //check intersections with every object in canvas
        if (activeObject.intersectsWithObject(targ)
            || activeObject.isContainedWithinObject(targ)
            || targ.isContainedWithinObject(activeObject)) {
            //objects are intersecting - deny saving last non-intersection position and break loop
            if (typeof activeObject.lastLeft === 'number') {
                activeObject.left = activeObject.lastLeft;
                activeObject.top = activeObject.lastTop;
                activeObject.refreshLast = false;
                return;
            }
        }
        else {
            activeObject.refreshLast = true;
        }
    });

    if (activeObject.refreshLast) {
        //save last non-intersecting position if possible
        activeObject.lastLeft = activeObject.left
        activeObject.lastTop = activeObject.top;
    }
}

function findNewPos(distX, distY, target, obj) {
    // See whether to focus on X or Y axis
    if (Math.abs(distX) > Math.abs(distY)) {
        if (distX > 0) {
            setActualRight(actualLeft(obj), target);
        } else {
            setActualLeft(actualRight(obj), target);
        }
    } else {
        if (distY > 0) {
            setActualBottom(actualTop(obj), target);
        } else {
            setActualTop(actualBottom(obj), target);
        }
    }
}
function centerX(obj) { var bound = obj.getBoundingRect(); return bound.left + bound.width / 2; }
function centerY(obj) { var bound = obj.getBoundingRect(); return bound.top + bound.height / 2; }
function actualWidth(obj) { return obj.getBoundingRect().width; }
function actualHeight(obj) { return obj.getBoundingRect().height; }
function actualLeft(obj) { return obj.getBoundingRect().left; }
function actualRight(obj) { var bound = obj.getBoundingRect(); return bound.left + bound.width; }
function actualTop(obj) { return obj.getBoundingRect().top; }
function actualBottom(obj) { var bound = obj.getBoundingRect(); return bound.top + bound.height; }
function mod(n, m) { if (n < 0) n = n + m; return ((n % m) + m) % m; }
function setActualLeft(leftValue, obj) {
    var angleRot = obj.get('angle');
    switch (angleRot) {
        case 0: obj.left = leftValue; break;
        case 90: obj.left = leftValue + obj.height; break;
        case 180: obj.left = leftValue + obj.width; break;
        case 270: obj.left = leftValue; break;
        default: break;
    }
}
function setActualTop(topValue, obj) {
    var angleRot = obj.get('angle');
    switch (angleRot) {
        case 0: obj.top = topValue; break;
        case 90: obj.top = topValue; break;
        case 180: obj.top = topValue + obj.height; break;
        case 270: obj.top = topValue + obj.width; break;
        default: break;
    }
}
function setActualRight(rightValue, obj) {
    var angleRot = obj.get('angle');
    switch (angleRot) {
        case 0: obj.left = rightValue - obj.width; break;
        case 90: obj.left = rightValue; break;
        case 180: obj.left = rightValue; break;
        case 270: obj.left = rightValue - obj.height; break;
        default: break;
    }
}
function setActualBottom(bottomValue, obj) {
    var angleRot = obj.get('angle');
    switch (angleRot) {
        case 0: obj.top = bottomValue - obj.height; break;
        case 90: obj.top = bottomValue - obj.width; break;
        case 180: obj.top = bottomValue; break;
        case 270: obj.top = bottomValue; break;
        default: break;
    }
}
function Move(canvas, target) {
    // Don't allow objects off the canvas
    if (actualLeft(target) < snap) {
        setActualLeft(0, target);
    }
    if (actualTop(target) < snap) {
        setActualTop(0, target);
    }
    if (actualRight(target) > (canvasWidth - snap)) {
        setActualRight(canvasWidth, target);
    }
    if (actualBottom(target) > (canvasHeight - snap)) {
        setActualBottom(canvasHeight, target);
    }
    //intersectingCheck(target);

    // Loop through objects
    canvas.forEachObject(function (obj) {
        if (obj === target) return;
        if (obj.get('type') === 'line') return;
        if (obj.get('name') === 'pallet') return;

        // If objects intersect
        if (target.isContainedWithinObject(obj) || target.intersectsWithObject(obj) || obj.isContainedWithinObject(target)) {

            var distX = centerX(obj) - centerX(target);
            var distY = centerY(obj) - centerY(target);

            // Set new position
            findNewPos(distX, distY, target, obj);
        }

        target.setCoords();

        if (snap > 1) {
            // Snap objects to each other horizontally
            // If bottom points are on same Y axis
            if (Math.abs(actualBottom(target) - actualBottom(obj)) < snap) {
                // Snap target BL to object BR
                if (Math.abs(actualLeft(target) - actualRight(obj)) < snap) {
                    setActualLeft(actualRight(obj), target);
                    setActualBottom(actualBottom(obj), target);
                }
                // Snap target BR to object BL
                if (Math.abs(actualRight(target) - actualLeft(obj)) < snap) {
                    setActualRight(actualLeft(obj), target);
                    setActualBottom(actualBottom(obj), target);
                }
            }

            // If top points are on same Y axis
            if (Math.abs(actualTop(target) - actualTop(obj)) < snap) {
                // Snap target TL to object TR
                if (Math.abs(actualLeft(target) - actualRight(obj)) < snap) {
                    setActualLeft(actualRight(obj), target);
                    setActualTop(actualTop(obj), target);
                }
                // Snap target TR to object TL
                if (Math.abs(actualRight(target) - actualLeft(obj)) < snap) {
                    setActualRight(actualLeft(obj), target);
                    setActualTop(actualTop(obj), target);
                }
            }

            // Snap objects to each other vertically
            // If right points are on same X axis
            if (Math.abs(actualRight(target) - actualRight(obj)) < snap) {
                // Snap target TR to object BR
                if (Math.abs(actualTop(target) - actualBottom(obj)) < snap) {
                    setActualRight(actualRight(obj), target);
                    setActualTop(actualBottom(obj), target);
                }
                // Snap target BR to object TR
                if (Math.abs(actualBottom(target) - actualTop(obj)) < snap) {
                    setActualRight(actualRight(obj), target);
                    setActualBottom(actualTop(obj), target);
                }
            }

            // If left points are on same X axis
            if (Math.abs(actualLeft(target) - actualLeft(obj)) < snap) {
                // Snap target TL to object BL
                if (Math.abs(actualTop(target) - actualBottom(obj)) < snap) {
                    setActualLeft(actualLeft(obj), target);
                    setActualTop(actualBottom(obj), target);
                }
                // Snap target BL to object TL
                if (Math.abs(actualTop(target) - actualTop(obj)) < snap) {
                    setActualLeft(actualLeft(obj), target);
                    setActualBottom(actualTop(obj), target);
                }
            }
        }
    });
    target.setCoords();

    // If objects still overlap
    var outerAreaLeft = null,
        outerAreaTop = null,
        outerAreaRight = null,
        outerAreaBottom = null;

    canvas.forEachObject(function (obj) {
        if (obj === target) return;
        if (obj.get('name') === 'pallet') return;
        if (obj.get('type') === 'line') return;

        if (target.isContainedWithinObject(obj)
            || obj.isContainedWithinObject(target)) {

            var intersectLeft = null,
                intersectTop = null,
                intersectWidth = null,
                intersectHeight = null,
                intersectSize = null,
                targetLeft = actualLeft(target),
                targetRight = actualRight(target),
                targetTop = actualTop(target),
                targetBottom = actualBottom(target),
                objectLeft = actualLeft(obj),
                objectRight = actualRight(obj),
                objectTop = actualTop(obj),
                objectBottom = actualBottom(obj);

            // Find intersect information for X axis
            if (targetLeft >= objectLeft && targetLeft <= objectRight) {
                intersectLeft = targetLeft;
                intersectWidth = actualWidth(obj) - (intersectLeft - objectLeft);
            } else if (objectLeft >= targetLeft && objectLeft <= targetRight) {
                intersectLeft = objectLeft;
                intersectWidth = actualWidth(target) - (intersectLeft - targetLeft);
            }
            // Find intersect information for Y axis
            if (targetTop >= objectTop && targetTop <= objectBottom) {
                intersectTop = targetTop;
                intersectHeight = actualHeight(obj) - (intersectTop - objectTop);
            } else if (objectTop >= targetTop && objectTop <= targetBottom) {
                intersectTop = objectTop;
                intersectHeight = actualHeight(target) - (intersectTop - targetTop);
            }
            // Find intersect size (this will be 0 if objects are touching but not overlapping)
            if (intersectWidth > 0 && intersectHeight > 0) {
                intersectSize = intersectWidth * intersectHeight;
            }
            // Set outer snapping area
            if (actualLeft(obj) < outerAreaLeft || outerAreaLeft === null) {
                outerAreaLeft = actualLeft(obj);
            }
            if (actualTop(obj) < outerAreaTop || outerAreaTop === null) {
                outerAreaTop = actualTop(obj);
            }
            if ((actualRight(obj)) > outerAreaRight || outerAreaRight === null) {
                outerAreaRight = actualRight(obj);
            }
            if ((actualBottom(obj)) > outerAreaBottom || outerAreaBottom === null) {
                outerAreaBottom = actualBottom(obj);
            }
            // If objects are intersecting, reposition outside all shapes which touch
            if (intersectSize) {
                var distX = (outerAreaRight / 2) - (actualRight(target) / 2);
                var distY = (outerAreaBottom / 2) - (actualBottom(target) / 2);
                // Set new position
                findNewPos(distX, distY, target, obj);
            }
        }
    });
}
