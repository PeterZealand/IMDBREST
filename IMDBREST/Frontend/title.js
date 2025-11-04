Vue.createApp({
    data(){
        return{
            apiBase: "http://localhost:5041",
            baseURI:null,
            title:null,
            crewDirectors:[],
            crewWriters:[],
        }
    },
    async created(){
        this.readQuery()
        this.getDirectors()
        this.getWriters()
    },
    methods:{
        buildUrl(pathAndQuery) {
            const base = (this.apiBase || "").replace(/\/+$/, "");
            return base + pathAndQuery;
        },
        readQuery(){
            const urlParams = new URLSearchParams(window.location.search)
            const rawTitle = urlParams.get('title')
            this.baseURI = urlParams
            const decodedTitle = rawTitle ? decodeURIComponent(rawTitle) : 'No Title Found'
            console.log(decodedTitle)
            this.title = decodedTitle
        },
        async getActors(){
        },
        async getDirectors(){
            const url = this.buildUrl(`/api/Directors/Title?title=${this.title}`)
            const res = await axios.get(url)
            this.crewDirectors = res.data
        },
        async getWriters(){
            const url = this.buildUrl(`/api/Writers/Title?title=${this.title}`)
            const res = await axios.get(url)
            this.crewWriters = res.data
        },
    }
}).mount("#title")
