//https://plotly.com/javascript/streaming/

//var t = new Date();


/*
var cnt = 0;
var interval = setInterval(function()
{
	Plotly.extendTraces('plot', {
	x: [[rand()]],
	y: [[rand()]]
	}, [0])
	if(++cnt === 100) clearInterval(interval);
}, 300);
*/





const element_output = document.getElementById("out");
const element_form = document.getElementById("form");
const element_plot = document.getElementById("plot");

const layout = 
{
	showlegend: true,
};
Plotly.newPlot(element_plot, [], layout);

//console.log(element_plot);


function ws_sub(socket, producer_id, mode)
{
	assert(socket instanceof WebSocket);
	assert(Number.isInteger(producer_id));
	assert(Number.isInteger(mode));
	var buffer = new ArrayBuffer(8);
	var dv = new DataView(buffer);
	dv.setInt32(0, producer_id, true);
	dv.setInt32(4, mode, true);
	console.log("Websocket send: ", buf2hex(buffer), " to ", socket.url);
	socket.send(buffer);
}


//When method = 0: Unsubscribe
//When method = 1: Subscribe
//When method = 2: Toggle subscription
function subscribe(socket, producer_id, longname, method)
{
	assert(socket instanceof WebSocket);
	assert(Number.isInteger(producer_id));
	assert(Number.isInteger(method));
	//Check if this is already subscribed:
	const trace_index = element_plot.data.findIndex(x => x.producer_id === producer_id);
	if (trace_index >= 0)
	{
		if ((method == 2) || (method == 0))
		{
			Plotly.deleteTraces(element_plot, trace_index);
			ws_sub(socket, producer_id, 0);
			console.log("Unsubscribe: " + producer_id + ". Delete trace: " +  trace_index);
		}
		else
		{
			console.log("The producer " + producer_id + " is already subscribed");
		}
	}
	else if ((method == 2) || (method == 1))
	{
		ws_sub(socket, producer_id, 1);
		var trace =
		{
			visible: true,
			x: [null],
			y: [null],
			name: longname ? longname : ("Producer " + producer_id),
			type: 'scatter',
			producer_id: producer_id //Custom field. Does not belong to Plotly.
		};
		Plotly.addTraces(element_plot, trace);
		const trace_index = element_plot.data.findIndex(x => x.producer_id === producer_id);
		console.log("Subscribe: " + producer_id + ". Adding trace: " + trace_index);
	}
}







function ws_connect(url)
{
	console.log("WebSocket open:", url);
	var socket = new WebSocket(url);
	socket.binaryType = "arraybuffer";
	socket.onopen = function (event)
	{
		console.log("WebSocket onopen: ", url, event);
		hashchange1();
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
		const view = new DataView(event.data);
		const producer_id = view.getInt32(0, true);
		const time = new Date(Number(view.getBigInt64(4, true)));
		const value = view.getFloat32(12, true);
		console.log(buf2hex(event.data), producer_id, time, value);
		const trace_index = element_plot.data.findIndex(x => x.producer_id === producer_id);
		Plotly.extendTraces('plot', {
			x: [[time.toISOString()]],
			y: [[value]]
		}, [trace_index]);
	};
	return socket;
};



var url = ws_url("/ws/sub/producer");
var socket = ws_connect(url);



element_form.onsubmit = (event) =>
{
	event.preventDefault();
	const producer_id = Number(event.target.producer_id.value);
	const producer_longname = event.target.producer_id.querySelector('option[value="'+producer_id+'"]').innerText;
	//Fuckery:
	var f = location.hash.substring(1).split(",");
	if (f[0] == ""){f = [];}
	var i = f.indexOf(""+producer_id);
	if (i >= 0)
	{
		f.splice(i, 1);
	}
	else
	{
		f.push(""+producer_id);
	}
	location.hash = f.join(",");
}

function hashchange1()
{
	var f = location.hash.substring(1).split(",");
	console.log(f);
	if (f.includes('a'))
	{
		element_form.remove(); 
	}
	for(let i = 0; i < element_plot.data.length; ++i)
	{
		let producer_id = element_plot.data[i].producer_id;
		if (f.includes(""+producer_id) == false)
		{
			subscribe(socket, Number(producer_id), "", 0);
		}
	}
	for(let i = 0; i < f.length; ++i)
	{
		if (f[i] == ""){continue;}//Fuckery
		var j = Number(f[i]);
		if (isNaN(j) == false)
		{
			subscribe(socket, Number(j), "", 1);
		}
	}
}

window.addEventListener('hashchange', hashchange1, false);


