{
	function form_graphql_callback(event)
	{
		event.preventDefault();
		var query = "mutation{" + event.srcElement.getAttribute("graphql") + "(";
	
		{
			const data = new FormData(event.srcElement)
			console.log(data);
			for (var pair of data.entries()) {
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
			xhr.onload = function () {
				var r = JSON.parse(xhr.response);
				if (r.data === null) { return; }
				//TODO: Generate better log
				if (event.srcElement.elements.log) {
					event.srcElement.elements.log.value = xhr.response;
				}
			};
			xhr.onerror = function () {
				console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
			}
			var data = JSON.stringify({ "query": query });
			xhr.send(data);
		}
	}
	const elements_form = document.querySelectorAll('form[graphql]');
	//console.log(forms);
	for (i = 0; i < elements_form.length; ++i)
	{
		elements_form[i].addEventListener('submit', form_graphql_callback);
	}
}