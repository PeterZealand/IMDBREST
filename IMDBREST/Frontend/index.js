const app = Vue.createApp({
    data() {
        return {
            apiBase: "http://localhost:5041",

            search: {
                titles: "",
                actors: "",
            },
            results: {
                titles: [],
                actors: [],
            },
            loading: {
                titles: false,
                actors: false,
            },
            error: {
                titles: "",
                actors: "",
            },
            debouncers: {
                titles: null,
                actors: null,
            },
            debounceMs: 350,
            minQueryLength: 2,
            actor:{
                primaryName:"",
                birhtYear:null,
                deathYear:null,
            },
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
        };
    },
    async created(){
        this.getTitleTypes()
    },

    methods: {
        buildUrl(pathAndQuery) {
            const base = (this.apiBase || "").replace(/\/+$/, "");
            return base + pathAndQuery;
        },
        insertNewActor(){
        },

        // Titles
        onInputTitles() {
            clearTimeout(this.debouncers.titles);
            if (this.search.titles.length < this.minQueryLength) {
                this.results.titles = [];
                this.error.titles = "";
                return;
            }
            this.debouncers.titles = setTimeout(() => this.searchTitles(), this.debounceMs);
        },

        async searchTitles(immediate = false) {
            const q = this.search.titles
            if (!q || q.length < this.minQueryLength) {
                if (immediate) {
                    this.results.titles = [];
                    this.error.titles = "";
                }
                return;
            }
            this.loading.titles = true;
            this.error.titles = "";
            try {
                const url = this.buildUrl(`/api/Titles/Name?titleName=${encodeURIComponent(q)}`);
                const resp = await fetch(url, { method: "GET" });

                if (resp.status === 204) {
                    this.results.titles = [];
                    return;
                }
                if (!resp.ok) {
                    throw new Error(`HTTP ${resp.status}`);
                }
                const data = await resp.json();
                this.results.titles = Array.isArray(data) ? data : [];
            } catch (e) {
                this.results.titles = [];
                this.error.titles = `Failed to load titles: ${e?.message || e}`;
            } finally {
                this.loading.titles = false;
            }
        },

        clearTitles() {
            this.search.titles = "";
            this.results.titles = [];
            this.error.titles = "";
            clearTimeout(this.debouncers.titles);
        },

        // Actors
        onInputActors() {
            clearTimeout(this.debouncers.actors);
            if (this.search.actors.length < this.minQueryLength) {
                this.results.actors = [];
                this.error.actors = "";
                return;
            }
            this.debouncers.actors = setTimeout(() => this.searchActors(), this.debounceMs);
        },

        async searchActors(immediate = false) {
            const q = this.search.actors
            if (!q || q.length < this.minQueryLength) {
                if (immediate) {
                    this.results.actors = [];
                    this.error.actors = "";
                }
                return;
            }
            this.loading.actors = true;
            this.error.actors = "";
            try {
                const url = this.buildUrl(`/api/Actors/Name?actorName=${encodeURIComponent(q)}`);
                const resp = await fetch(url, { method: "GET" });

                // Actors endpoint returns 200 with [] for no results, but handle 204 just in case
                if (resp.status === 204) {
                    this.results.actors = [];
                    return;
                }
                if (!resp.ok) {
                    throw new Error(`HTTP ${resp.status}`);
                }
                const data = await resp.json();
                this.results.actors = Array.isArray(data) ? data : [];
            } catch (e) {
                this.results.actors = [];
                this.error.actors = `Failed to load actors: ${e?.message || e}`;
            } finally {
                this.loading.actors = false;
            }
        },

        clearActors() {
            this.search.actors = "";
            this.results.actors = [];
            this.error.actors = "";
            clearTimeout(this.debouncers.actors);
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
        startCreateNewTitleMethod(){
            this.startCreateNewTitle = !this.startCreateNewTitle

            if(this.startCreateNewTitle){
                this.getTitleTypes()
                this.getGenres()
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
    },
});

app.mount("#app");
