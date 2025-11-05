const app = Vue.createApp({
    data() {
        return {
            apiBase: "http://localhost:5041",

            pagination:{
                currentPage:1,
                perPage: 35
            },
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
            titleToRemove:null,
        };
    },
    async created(){
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
        startCreateNewTitleMethod(){
            location.href = "./createNewTitlePage.html"
        },
        startCreateNewActorMethod(){
            location.href = "./createNewActorPage.html"
        },

        async getMoreInfoTitle(title){
            const encodedTitle = encodeURIComponent(title)

            location.href = './title.html?title=' + encodedTitle
        },
        async getMoreInfoActor(actor){
            const encodedActor = encodeURIComponent(actor)

            location.href = './actor.html?actor=' + encodedActor
        },
        async removeTitle(title){
                this.titleToRemove = title
            if(window.confirm(`Delete '${this.titleToRemove}'?`)){
                const getTitleIdUrl = this.buildUrl(`/api/Titles/Id?titleName=${this.titleToRemove}`)

                const res = await axios.get(getTitleIdUrl)
                console.log(res.data)
                const deleteTitleUrl = this.buildUrl(`/api/Titles/${res.data}`)
                const deleteRes = await axios.delete(deleteTitleUrl)
                window.alert(`Deleted ${this.titleToRemove}`)
                this.titleToRemove = null
                location.reload()
            }
        }
    },
});

app.mount("#app");
