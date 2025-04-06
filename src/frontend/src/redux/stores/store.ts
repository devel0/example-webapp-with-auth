// generic: https://redux.js.org/tutorials/quick-start
// TS specific: https://react-redux.js.org/tutorials/typescript-quick-start

import {  combineReducers,  configureStore,} from "@reduxjs/toolkit";
import { enableMapSet } from 'immer'
import globalReducer from "../slices/globalSlice";

enableMapSet()

export const store = configureStore({
  reducer: combineReducers({
    global: globalReducer,
    // add more         
  }),
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      // serializableCheck: false,
    })
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;

// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch;
