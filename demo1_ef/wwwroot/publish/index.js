




/*

	public int id { get; set; }
	public int layout_id { get; set; }
	public int producer_id { get; set; }
	public int byteoffset { get; set; }
	public virtual Layout layout { get; set; }
*/


function show_byte_array(memlocs, length, etable, element_hexs2)
{
	assert(memlocs instanceof Array);
	assert(etable instanceof HTMLElement);
	assert(element_hexs2 instanceof Array);//array of HTMLElement
	let row1 = etable.insertRow(-1);
	etable._row_hex = etable.insertRow(-1);
	let row0 = etable.insertRow(-1);
	let row3 = etable.insertRow(-1);
	for (let i = 0; i < length; ++i)
	{
		let cell;
		cell = row1.insertCell(-1);
		cell.innerText = String(i).padStart(2, '0');
		cell = etable._row_hex.insertCell(-1);
		cell.innerText = String(0).padStart(2, '0');
	}
	
	let j = 0;
	let i;
	for (i = 0; i < length; ++i)
	{
		let cell;
		if (memlocs[j].byteoffset == i)
		{
			cell = row0.insertCell(-1);
			cell.colSpan = 4;
			cell.innerText = String(memlocs[j].id);
			cell.style.backgroundColor = random_hsla(""+j);
			cell.style.textAlign = "center";
			cell = row3.insertCell(-1);
			cell.colSpan = 4;
			cell.innerText = "0";
			element_hexs2[j] = cell;
		}
		if (memlocs[j].byteoffset <= i && memlocs[j].byteoffset <= i+3)
		{
			//row1.cells[i].style.backgroundColor = random_hsla(""+j);
		}
		else
		{
			cell = row0.insertCell(-1);
			cell.innerText = "";
			cell = row3.insertCell(-1);
			cell.innerText = "";
		}
		if (memlocs[j].byteoffset+3 <= i)
		{
			j++;
		}
		if (j >= memlocs.length){i++;break;}
	}
	for (; i < length; ++i)
	{
		cell = row0.insertCell(-1);
		cell.innerText = "";
		cell = row3.insertCell(-1);
		cell.innerText = "";
	}
}


function show_inputs(layout_id, memlocs, subs, sendbuffer, element_tbody, e_table_bytes, element_inputs, element_hexs1, element_hexs2)
{
	assert(typeof layout_id == 'number');
	assert(memlocs instanceof Array);
	assert(subs instanceof Array);
	assert(sendbuffer instanceof ArrayBuffer);
	assert(element_tbody instanceof HTMLElement);
	assert(e_table_bytes instanceof HTMLElement);
	assert(element_inputs instanceof Array);//array of HTMLElement
	assert(element_hexs1 instanceof Array);//array of HTMLElement
	assert(element_hexs2 instanceof Array);//array of HTMLElement
	//assert(etable instanceof HTMLElement);
	for (let i = 0; i < memlocs.length; ++i)
	{
		let row = element_tbody.insertRow(-1);
		let cell;
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].id;
		cell.style.backgroundColor = random_hsla(""+i);
		cell = row.insertCell(-1);
		cell.innerText = memlocs[i].producer_id;
		cell.classList.add("cell");
		cell.onclick = (x) => 
		{
			var j = subs.indexOf(x.target.innerText);
			if (j >= 0)
			{
				subs.splice(j, 1);
				x.target.classList.remove("active");
			}
			else
			{
				subs.push(x.target.innerText);
				x.target.classList.add("active");
			}
			document.getElementById('iframe').src = "/subscribe/#a,"+subs.join(",");
			console.log(subs, j, x.target.innerText, document.getElementById('iframe').src);
		}
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
			e.onkeyup = (x) => 
			{
				update_buffer(sendbuffer, layout_id, memlocs, element_inputs);
				showhex2(sendbuffer, e_table_bytes._row_hex);
				showhex(memlocs, element_inputs, element_hexs1, element_hexs2);
			}
			cell.appendChild(e);
			element_inputs.push(e);
		}
		cell = row.insertCell(-1);
		{
			cell.innerText = 0;
			element_hexs1.push(cell);
		}
	}
}


// Side effect
function showhex(memlocs, element_inputs, element_hexs1, element_hexs2)
{
	assert(memlocs instanceof Array);
	assert(element_inputs instanceof Array);//array of HTMLElement
	assert(element_hexs1 instanceof Array);//array of HTMLElement
	assert(element_hexs2 instanceof Array);//array of HTMLElement
	var buffer = new ArrayBuffer(4);
	const view = new DataView(buffer);
	for (let i = 0; i < memlocs.length; ++i)
	{
		var value = Number(element_inputs[i].value);
		//console.log(value);
		view.setFloat32(0, value);
		element_hexs1[i].innerText = buf2hex(buffer);
		element_hexs2[i].innerText = value.toPrecision(6);
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
	assert(buf instanceof ArrayBuffer);
	assert(tr instanceof HTMLElement);
	var u = new Uint8Array(buf)
	for (let i = 0; i < tr.cells.length; ++i)
	{
		tr.cells[i].innerText = u[i].toString(16).toUpperCase();
	}
}


// Pure function
function pub(socket, layout_id, memlocs, element_inputs)
{
	assert(element_inputs instanceof Array);
	assert(socket instanceof WebSocket);
	assert(typeof layout_id == 'number');
	const bytesize = memlocs[memlocs.length-1].byteoffset + 4;
	let buffer = new ArrayBuffer(bytesize);
	let view = new DataView(buffer);
	//console.log(element_inputs);
	view.setInt32(0, layout_id);
	for (let i = 0; i < memlocs.length; ++i)
	{
		const value = Number(element_inputs[i].value);
		//console.log(value);
		view.setFloat32(memlocs[i].byteoffset, value);
	}
	//console.log(view, buffer);
	console.log("Sending: ", buf2hex(buffer));
	socket.send(buffer);
}


function ws_connect(url)
{
	console.log("WebSocket open:", url);
	let socket = new WebSocket(url);
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







class load
{
	constructor(layout_id)
	{
		this.element_hexs1 = [];
		this.element_hexs2 = [];
		this.element_inputs = [];
		this.sendbuffer = null;
		let subs = [];
		let element_tbody = document.getElementById("tbody");
		let e_table_bytes = document.getElementById("bytes");
		let element_button_pub = document.getElementById("pub");
		let element_button_pubr = document.getElementById("pubr");
		let url = ws_url("/ws/pub/layout");
		let socket = ws_connect(url);
		//let layout_id = 4;
		let query = "query{memlocs(layout_id:" + layout_id + "){id layout_id producer_id byteoffset producer{quantity{name}}}}";
		let xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = () =>
		{
			console.log(this);
			let r = JSON.parse(xhr.response);
			if (r.data === null) { return; }
			let memlocs = r.data.memlocs;
			console.log(memlocs);
			let bytesize = memlocs[memlocs.length - 1].byteoffset + 4;
			this.sendbuffer = new ArrayBuffer(bytesize);
			show_inputs(layout_id, memlocs, subs, this.sendbuffer, element_tbody, e_table_bytes, this.element_inputs, this.element_hexs1, this.element_hexs2);
			show_byte_array(memlocs, this.sendbuffer.byteLength, e_table_bytes, this.element_hexs2);
			element_button_pub.onclick = (x) => { pub(socket, layout_id, memlocs, this.element_inputs); };
			element_button_pubr.onclick = (x) => 
			{
				for(let i = 0; i < this.element_inputs.length; ++i)
				{
					this.element_inputs[i].value = Math.random();
					this.element_inputs[i].onkeyup();
				}
				pub(socket, layout_id, memlocs, this.element_inputs);
			};
		};
		xhr.onerror = function () {
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		};
		let data = JSON.stringify({ "query": query });
		xhr.send(data);
	}
}






function hashchange1()
{
	var f = location.hash.substring(1);
	console.log(f);
	new load(Number(f));
}

window.addEventListener('hashchange', hashchange1, false);
hashchange1();