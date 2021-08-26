
var e_table_bytes = document.getElementById("bytes");
var e_tr_bytes;
var element_tbody = document.getElementById("tbody");
var element_button_pub = document.getElementById("pub");
var element_inputs = [];
var element_hexs = [];
var element_hexs2 = [];
var layout_id = 4;
var socket;
var sendbuffer = new ArrayBuffer(100);
/*

	public int id { get; set; }
	public int layout_id { get; set; }
	public int producer_id { get; set; }
	public int byteoffset { get; set; }
	public virtual Layout layout { get; set; }
*/


function show_byte_array(memlocs)
{
	var row1 = e_table_bytes.insertRow(-1);
	e_tr_bytes = e_table_bytes.insertRow(-1);
	var row3 = e_table_bytes.insertRow(-1);
	for (let i = 0; i < 50; ++i)
	{
		var cell;
		cell = row1.insertCell(-1);
		cell.innerText = String(i).padStart(2, '0');
		cell = e_tr_bytes.insertCell(-1);
		cell.innerText = String(0).padStart(2, '0');
	}
	
	let j = 0;
	let i;
	for (i = 0; i < 50; ++i)
	{
		var cell;
		if (memlocs[j].byteoffset == i)
		{
			cell = row3.insertCell(-1);
			cell.colSpan = 4;
			cell.innerText = String(i).padStart(2, '0');
			element_hexs2[j] = cell;
		}
		if (memlocs[j].byteoffset <= i && memlocs[j].byteoffset <= i+3)
		{
			row1.cells[i].style.backgroundColor = random_hsla(""+j);
		}
		else
		{
			cell = row3.insertCell(-1);
			cell.innerText = String(i).padStart(2, '0');
		}
		if (memlocs[j].byteoffset+3 <= i)
		{
			j++;
		}
		if (j >= memlocs.length){i++;break;}
	}
	for (; i < 50; ++i)
	{
		cell = row3.insertCell(-1);
		cell.innerText = String(i).padStart(2, '0');
	}
}


function show_inputs(memlocs)
{
	for (let i = 0; i < memlocs.length; ++i)
	{
		var row = element_tbody.insertRow(-1);
		var cell;
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].id;
		cell.style.backgroundColor = random_hsla(""+i);
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].producer_id;
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].producer.quantity.name
		//cell = row.insertCell(0);
		//cell.innerText = memlocs[i].layout_id;
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].byteoffset;
		cell = row.insertCell(-1);
		{
			let e = document.createElement("input");
			e.setAttribute("type", "text");
			e.setAttribute("value", "0");
			e.onchange = (x) => 
			{
				update_buffer(sendbuffer, layout_id, memlocs, element_inputs);
				showhex2(sendbuffer, e_tr_bytes);
				showhex(memlocs);
			}
			cell.appendChild(e);
			element_inputs.push(e);
		}
		cell = row.insertCell(-1);
		{
			cell.innerText = 0;
			element_hexs.push(cell);
		}
	}
}


// Side effect
function showhex(memlocs)
{
	var buffer = new ArrayBuffer(4);
	const view = new DataView(buffer);
	for (let i = 0; i < memlocs.length; ++i)
	{
		var value = Number(element_inputs[i].value);
		//console.log(value);
		view.setFloat32(0, value);
		element_hexs[i].innerText = buf2hex(buffer);
		element_hexs2[i].innerText = value;
	}
}


// Pure function
function update_buffer(buf, layout_id, memlocs, inputs)
{
	const view = new DataView(buf);
	view.setInt32(0, layout_id);
	for (let i = 0; i < memlocs.length; ++i)
	{
		var value = Number(inputs[i].value);
		view.setFloat32(memlocs[i].byteoffset, value);
	}
}


// Pure function
function showhex2(buf, tr)
{
	var u = new Uint8Array(buf)
	//console.log(e_tr_bytes.cells);
	for (let i = 0; i < tr.cells.length; ++i)
	{
		tr.cells[i].innerText = u[i].toString(16);
	}
}


// Side effect
function pub(memlocs)
{
	var buffer = new ArrayBuffer(100);
	const view = new DataView(buffer);
	//console.log(memlocs);
	//console.log(element_inputs);
	view.setInt32(0, layout_id);
	for (let i = 0; i < memlocs.length; ++i)
	{
		var value = Number(element_inputs[i].value);
		//console.log(value);
		view.setFloat32(memlocs[i].byteoffset, value);
	}
	console.log(view, buffer);
	socket.send(buffer);
}


function ws_connect(url)
{
	console.log("WebSocket open:", url);
	socket = new WebSocket(url);
	socket.binaryType = "arraybuffer";
	socket.onopen = function (event)
	{
		console.log("WebSocket onopen: ", url, event);
	};
	socket.onclose = function (event)
	{
		console.log("WebSocket onclose: ", event);
	};
	socket.onerror = function (event)
	{
		console.warn("WebSocket onerror: ", event);
		//element_output.innerText = "Unknown Error. You need to be /Siteadmin to connect to websocket.";
	};
	socket.onmessage = function (event)
	{
		console.warn("WebSocket onmessage: ", event);
	};
	return socket;
};








{
	var query = "query{memlocs(layout_id:"+layout_id+"){id layout_id producer_id byteoffset producer{quantity{name}}}}";
	var xhr = new XMLHttpRequest();
	xhr.open("POST", window.location.origin + "/graphql", true);
	xhr.setRequestHeader("Content-Type", "application/json");
	xhr.onload = function ()
	{
		var r = JSON.parse(xhr.response);
		if (r.data === null){return;}
		let memlocs = r.data.memlocs;
		console.log(memlocs);
		show_inputs(memlocs);
		show_byte_array(memlocs);
		element_button_pub.onclick = (x) => {pub(memlocs)};
	};
	xhr.onerror = function()
	{
		console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
	}
	var data = JSON.stringify({"query": query});
	xhr.send(data);
}

{
	var url = ws_url("/ws/pub");
	var socket = ws_connect(url);
}



