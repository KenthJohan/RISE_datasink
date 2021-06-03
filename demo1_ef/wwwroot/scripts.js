//http://www.davejennifer.com/computerjunk/javascript/tail-dash-f.html


var mqtt_server_host = "ws://192.168.1.195:1884";




function swap32(val)
{
	return ((val & 0xFF) << 24)
		| ((val & 0xFF00) << 8)
		| ((val >> 8) & 0xFF00)
		| ((val >> 24) & 0xFF);
}




function req_arraybuffer(filename, from, to, callback)
{
	var meter = document.getElementById('value1');
	console.log("Loading file: " + filename);
	var xhr = new XMLHttpRequest();
	xhr.responseType = "arraybuffer";
	//xhr.overrideMimeType("application/octet-stream");
	xhr.open("GET", filename, true);
	xhr.setRequestHeader('Range', 'bytes=' + from + '-' + to);
	xhr.onprogress = (event) =>
	{
		meter.value = (100 * event.total / event.loaded);
		console.log("Downloaded ${event.loaded} of ${event.total} bytes");
	}
	xhr.onload = function (e)
	{
		var buffer = xhr.response;
		var f = new Float32Array(buffer, 0, buffer.byteLength / 4);
		var n = f.length / 4;
		var x = new Float32Array(n);
		var y = new Float32Array(n);
		var z = new Float32Array(n);
		for (var i = 0; i < n; i++)
		{
			x[i] = (f[i * 4 + 0]);
			y[i] = (f[i * 4 + 1]);
			z[i] = (f[i * 4 + 2]);
			//w[i] = (f[i * 4 + 3]);Light intensity
		}
		//console.log(f);
		//console.log(x);
		//console.log(y);
		//console.log(z);
		callback(x, y, z);
	};
	xhr.onerror = function (e)
	{
		console.error(e);
	};
	xhr.send();
}









function req_json(filename, callback)
{
	console.log("Requesting json from: " + filename);
	var xhr = new XMLHttpRequest();
	xhr.open("GET", filename, true);
	xhr.onload = function (e)
	{
		//console.log(xhr.responseText);
		var obj = JSON.parse(xhr.responseText);
		//console.log(obj);
		callback(obj);
	};
	xhr.onerror = function (e)
	{
		console.error(e);
	};
	xhr.send();
}









var quantity1dims = ["humidity", "temperature"];


var converters = {};
converters["timestamp"] = function(view)
{
	var r;
	r = view.getFloat64(0, true);
	return r;
}
converters["position_gcs"] = function(view)
{
	var r = [];
	r[0] = view.getFloat32(0, true);
	r[1] = view.getFloat32(4, true);
	return r;
}
converters["temperature"] = function(view)
{
	var r;
	r = view.getFloat32(0, true);
	return r;
}
converters["humidity"] = function(view)
{
	var r;
	r = view.getFloat32(0, true);
	return r;
}
converters["position_xyzw"] = function(view)
{
	var r = [];
	r[0] = view.getFloat32(0, true);
	r[1] = view.getFloat32(4, true);
	r[2] = view.getFloat32(8, true);
	r[3] = view.getFloat32(12, true);
	return r;
}


function convert_payload(components, compname, payload)
{
	//Warning! The payload does not start at byte 0!
	dataview = new DataView(payload.buffer, payload.byteOffset + components[compname].offset);
	return converters[compname](dataview);
}










var colors = 
{
	"timestamp":"rgb(210, 210, 210)",
	"position_xyzw":"rgb(212, 150, 202)",
	"position_gcs":"rgb(160, 230, 190)",
	"temperature":"rgb(255, 151, 151)",
	"humidity":"rgb(134, 161, 192)"
};




function generate_htmltable_series(etable, archetypes)
{
	var cell;
	var row;
	row = etable.insertRow(-1);
	cell = row.insertCell(-1);
	cell.innerHTML = "series";
	cell = row.insertCell(-1);
	cell.innerHTML = "component";
	cell = row.insertCell(-1);
	cell.innerHTML = "quantity";
	cell = row.insertCell(-1);
	cell.innerHTML = "n";
	cell = row.insertCell(-1);
	cell.innerHTML = "offset";
	cell = row.insertCell(-1);
	cell.innerHTML = "size";
	cell = row.insertCell(-1);
	cell.innerHTML = "endian";
	cell = row.insertCell(-1);
	cell.innerHTML = "value";
	for (let archetype in archetypes)
	{
		var components = archetypes[archetype].components;
		//console.log(i);
		for (let j in components)
		{
			row = etable.insertRow(-1);
			cell = row.insertCell(-1);
			cell.innerHTML = archetype;
			cell = row.insertCell(-1);
			cell.innerHTML = j;
			cell = row.insertCell(-1);
			cell.innerHTML = components[j].quantity;
			cell.style.backgroundColor = colors[components[j].quantity];
			cell = row.insertCell(-1);
			cell.innerHTML = components[j].n;
			cell = row.insertCell(-1);
			cell.innerHTML = components[j].offset;
			cell = row.insertCell(-1);
			cell.innerHTML = components[j].size;
			cell = row.insertCell(-1);
			cell.innerHTML = components[j].endian;
			cell = row.insertCell(-1);
			cell.innerHTML = 0;
			components[j]._cell = cell;
			/*
			cell.onclick = () => 
			{
				hej(name, j);
			}
			*/
		}
	}
	//var row = etable.insertRow(0);
}










function series_filter(series, filter)
{
	for (s in series)
	{
		if (filter[s] == null)
		{
			delete series[s];
			continue;
		}
		var components = series[s].components;
		for (c in components)
		{
			if (filter[s].includes(c) == false)
			{
				delete components[c];
			}
		}
	}
}