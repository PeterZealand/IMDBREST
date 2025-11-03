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
        startCreateNewTitleMethod(){
            location.href = "./createNewTitlePage.html"
        },

        async getMoreInfo(title){
            location.href = './title.html'
        },
    },
});

app.mount("#app");
