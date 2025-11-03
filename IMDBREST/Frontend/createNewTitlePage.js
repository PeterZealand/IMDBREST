Vue.createApp({
    data(){
        return{
            apiBase: "http://localhost:5041",

            title:{
                typeId:null,
                primaryTitle:null,
                originalTitle:null,
                titleType: null,
                isAdult:null,
                startYear: null,
                endYear: null,
                runtimeMinutes: null,
                genres:[]
            },
            startCreateNewTitle:false,
            titleTypes:[],
            genres:[],
            directors:[],
            writers:[]
        }
    },
    async created(){
    },
    async mounted(){
        this.getTitleTypes()
        this.getGenres()
    },
    methods:{
        buildUrl(pathAndQuery) {
            const base = (this.apiBase || "").replace(/\/+$/, "");
            return base + pathAndQuery;
        },
        async getTitleTypes(){
            try{
                const url = this.buildUrl(`/api/TitleTypes/`)
                const resp = await fetch(url,{method:"GET"})
                const data = await resp.json()
                this.titleTypes = Array.isArray(data) ? data : []
            }
            catch(error){
            }
        },
        async getGenres(){
            try{
                const url = this.buildUrl(`/api/Genres/`)
                const resp = await fetch(url,{method:"GET"})
                const data = await resp.json()
                this.genres = Array.isArray(data) ? data : []
            }
            catch(error){
            }
        },
        async getTypeId(){
            try{
                titleTypesUrl = this.buildUrl(`/api/TitleTypes/`)
                const getTypeId = await axios.get(titleTypesUrl + `Name?titleName=${this.title.titleType}`)
                console.log(getTypeId.data)
                this.title.typeId = getTypeId.data
            }
            catch(error){
            }
        },
        async insertNewTitle(){
            titlesUrl = this.buildUrl(`/api/Titles/`)

            newTitle = {
                id: 0,
                typeId: this.title.typeId,
                primaryTitle: this.title.primaryTitle,
                originalTitle: this.title.originalTitle,
                isAdult: String(this.title.isAdult).toLowerCase() === 'true',
                startYear: this.title.startYear,
                endYear: this.title.endYear,
                runtimeMinutes: this.title.runtimeMinutes,
                genres: this.genres
            }

            try{
                const res = await axios.post(titlesUrl,newTitle)
                window.alert("Title inserted")
            }
            catch(error){
                if(error.response){
                    console.error("400 Error Details:",error.response.data)
                    console.error("Status:",error.response.status)
                    window.alert("Insertion failed. Check console for details. Server message: "+JSON.stringify(error.response.data))
                }
                else if(error.request){
                    console.error("No response receieved:",error.request)
                }
                else{
                    console.error("Request setup error:",error.message)
                }
            }

            // location.reload()
        },
    }
}).mount("#createNewTitle")
