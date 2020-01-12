/**
 * @author bg357
 * Initial code in this map height generator created for cs4621
 */


//TODO: How does smoothness + poly-ness relates to freq, dim, and heightBound?
var p5 = new p5();


var scale = 35;                 //Size of the output in XZ
var poly = 1;                  //Number of polygons per 1x1 area
var smoothness = 7;             //jagged <= low val, high val => flat
var heightBound = 10;           //height values range from 0 to this value


var freq = scale/smoothness;    //Rate of change in the perlin noise
var dim = scale*poly;           //Size of stored array



var heightMap;
           
var pixwidth = 1;                //Width of each array value in the canvas

//TODO: Use these values to offset the terrain obj output by a given amount
//Use to support stitching later
//Edit @coordsInWorld to support center editing
var centerOffsetX = (scale/2);
var centerOffsetZ = (scale/2);


function populateMap(){
    heightMap = generateHeightmap();
    drawCanvas(heightMap);
    console.log("Populate with poly = "  + poly);
}

function outToFile(){
    var obj = arrToOBJString(heightMap);
    console.log(obj);

}

/**
 * Calculates map height value at an X,Z by 2D perlin noise
 */
function getHeight(ix, iz){
    //transform coords to 0-1 range floats
    //scale/2 is a hacky fix by adding half of scale to undo centering around 0
    var nx = (coordsInWorld(ix, centerOffsetX) + scale/2) /scale;  
    var nz = (coordsInWorld(iz, centerOffsetZ) + scale/2) /scale;

    num = p5.noise(nx*freq, nz*freq);
    num = Math.min(heightBound, num*heightBound);//Stretch the 0_1 output into 0_heightBound
    return num;
}


function logArray(arr){
    var out = "";

    for(var x = 0; x < dim; x++){
        var temp = "";
        for(var z = 0; z < dim; z++){
            temp += arr[x][z] + " ";
        }
        out += temp + "\n";
    }
    console.log(out);
}

/**
 * Initiate heightmap with the formula described in @getCanvas
 */
function generateHeightmap(){
    var arr = new Array(dim);
    for(var x = 0; x < dim; x++){
        arr[x] = [];
        for(var z = 0; z < dim; z++){
            arr[x][z] = getHeight(x,z);
        }
    }
    return arr;
}

/**
 * Draw the canvas based on the values in arr
 */
function drawCanvas(arr){
    var canvas = document.getElementById("acanvas");
    canvas.height = canvas.width = dim*pixwidth;
    var context = canvas.getContext("2d");

    for(x = 0; x < dim; x++) {
        for(y = 0; y < dim; y++) {
            var rgb = colorFromValue(arr[x][y]);
            context.fillStyle = "rgb(" + rgb[0] + "," + rgb[1] + "," + rgb[2] + ")";
            context.fillRect(x*pixwidth, y*pixwidth, pixwidth, pixwidth);
        }
    }
}

/**
 * Returns an RGB 3 length array with the pixel color corresponding to an array value
 * @param {*} x the value of arr to turn into a pixel colour
 */
function colorFromValue(x){
    //Get the value scaled to be from 0 to 255 (greyscale)
    var num = Math.min(255, (255 / heightBound) * x);
    return [num,num,num];
}






// /// //// //// ///// ////// 

/**
 * Converts the given heightmap to an obj file (string)
 * @param {*} arr The input heightmap
 */
function arrToOBJString(arr){
    var points = "";
    var faces = "";
    var normals = "";

    var faceNum = 1;

    //Add points to obj file
    //Iterate through each x then each z (x primary labeling order)
    for(z = 0; z < dim; z++){
        for(x = 0; x < dim; x++){

            //Add point to point list
            var p1 = [coordsInWorld(x, centerOffsetX), arr[x][z], coordsInWorld(z, centerOffsetX)]; 
            points += "v " + p1[0] + " " + p1[1] + " " + p1[2] + "\n";

            //When dealing with faces must not be last row of vertices
            //TODO: Fix vertex bounds without artifacts, currently over restricts
            if (z > 0 && z < arr[0].length-2 && x > 0 && x < arr[0].length-2){

                var p2 = [coordsInWorld(x, centerOffsetX), arr[x][z+1], coordsInWorld(z+1, centerOffsetX)];
                var p3 = [coordsInWorld(x+1, centerOffsetX), arr[x+1][z], coordsInWorld(z, centerOffsetX)];
                var p4 = [coordsInWorld(x+1, centerOffsetX), arr[x+1][z+1], coordsInWorld(z+1, centerOffsetX)];
                
                var norm1 = getFaceNormal(p1, p2, p4);
                normals+= "vn " + norm1[0] + " " + norm1[1] + " " + norm1[2] + "\n";
                
                var norm2 = getFaceNormal(p1, p4, p3);
                normals+= "vn " + norm2[0] + " " + norm2[1] + " " + norm2[2] + "\n";


                var p1num = getPointNumber(x, z, arr.length);
                var p2num = getPointNumber(x, z+1, arr.length);
                var p3num = getPointNumber(x+1, z, arr.length);
                var p4num = getPointNumber(x+1, z+1, arr.length);


                faces += "f " + p1num + "//" +faceNum + " " + p2num + "//"+faceNum+" " + p4num + "//" + faceNum + "\n";
                faceNum++;
                faces += "f " + p1num + "//" +faceNum + " " + p4num + "//"+faceNum + " " + p3num + "//" +faceNum + "\n";
                faceNum++;
            }
        }
    }

    //Output string to console
    return points + normals + faces
}


/**
 * Get the normal to a face based on three vertexes, CCW order 
 */
function getFaceNormal(v0, v1, v2){
    //V1 - V0, = A
    var U = [v1[0] - v0[0], v1[1] - v0[1], v1[2] - v0[2]]; 

    //V2 - V0, = B
    var V = [v2[0] - v0[0], v2[1] - v0[1], v2[2] - v0[2]];
    
    var normal = [0.0,0.0,0.0];

    normal[0] = (U[1] * V[2]) - (U[2] * V[1])
	normal[1] = (U[2] * V[0]) - (U[0] * V[2])
	normal[2] = (U[0] * V[1]) - (U[1] * V[0])

    var magnitude = Math.sqrt(normal[0]*normal[0] + normal[1]*normal[1] + normal[2]*normal[2]);

    normal [0] = normal[0]/magnitude;
    normal [1] = normal[1]/magnitude;
    normal [2] = normal[2]/magnitude;

    return normal;
}


function getPointNumber(x, z, w){
    return x + 1 + w*z;
}

function coordsInWorld(i, center){
    var throwaway = center;
    var centerIn_i = dim/2; //TODO: This is only valid for center 0
    return (i - centerIn_i) * (1/poly);
}