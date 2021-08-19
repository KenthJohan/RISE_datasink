function submit_callback(event)
{
	event.preventDefault();
	var query = "mutation{" + event.srcElement.getAttribute("graphql") + "(";

	const graphql_lists = event.srcElement.querySelectorAll('input[graphql_list]');
	for (i = 0; i < graphql_lists.length; ++i)
	{
		const datalist = event.srcElement.querySelector("#"+graphql_lists[i].getAttribute("list"));
		var op = datalist.querySelector('[value="'+graphql_lists[i].value+'"]');
		console.log(op);
		graphql_lists[i].value = op.getAttribute("value_id");
	}

	{
		const data = new FormData(event.srcElement)
		console.log(data);
		for (var pair of data.entries())
		{
			query += pair[0] + ":" + pair[1] + ",";
		}
		query = query.slice(0, -1);
		query += ")}";
		console.log(query);
	}

	{
		var f = Math.random();
		var xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = function ()
		{
			var r = JSON.parse(xhr.response);
			if (r.data === null){return;}
			//TODO: Generate better log
			if (event.srcElement.elements.log)
			{
				event.srcElement.elements.log.value = xhr.response;
			}
		};
		xhr.onerror = function()
		{
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		}
		var data = JSON.stringify({"query": query});
		xhr.send(data);
	}
}


{
	const forms = document.querySelectorAll('form[graphql]');
	//console.log(forms);
	for (i = 0; i < forms.length; ++i)
	{
		forms[i].addEventListener('submit', submit_callback);
	}
}




{
	const inputs = document.querySelectorAll('input[type="datetime-local"][now]');
	//console.log(inputs);
	for (i = 0; i < inputs.length; ++i)
	{
		const now = new Date();
		now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
		//now.setMilliseconds(null);
		inputs[i].value = now.toISOString();
	}
}



















{
	const inputs = document.querySelectorAll('input[graphql_list]');
	console.log(inputs);
	for (i = 0; i < inputs.length; ++i)
	{
		var datalist_id_name = "datalist_" + inputs[i].getAttribute("name");
		const query = "query{"+inputs[i].getAttribute("graphql_list")+"}";
		datalist = document.createElement('datalist');
		datalist.setAttribute("id", datalist_id_name);
		inputs[i].setAttribute("list", datalist_id_name);

		var xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = function ()
		{
			var r = JSON.parse(xhr.response);
			if (r.data === null){return;}
			var options = r.data.options;
			for (j = 0; j < options.length; ++j)
			{
				option = document.createElement('option');
				option.setAttribute("value", options[j].value);
				option.setAttribute("value_id", options[j].value_id);
				datalist.appendChild(option);
			}
		};
		xhr.onerror = function()
		{
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		}
		var data = JSON.stringify({"query": query});
		xhr.send(data);


		
		inputs[i].parentNode.insertBefore(datalist, inputs[i].nextSibling);
	}
}



