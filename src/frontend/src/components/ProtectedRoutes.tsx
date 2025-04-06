// https://github.com/remix-run/react-router/issues/10637#issuecomment-1802180978

import { APP_URL_Login } from '../constants/general';
import { GlobalState } from '../redux/states/GlobalState'
import { Navigate } from 'react-router'
import { Outlet } from "react-router-dom";
import { useAppSelector } from '../redux/hooks/hooks'
import Layout from './Layout';

const ProtectedRoutes = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)    

    const loginRedirectUrlFrom = () => {
        if (location.pathname !== APP_URL_Login())
            return encodeURIComponent(location.pathname)
    }

    return global.currentUser
        ?
        <Layout child={<Outlet />} />
        :
        <Navigate to={APP_URL_Login(loginRedirectUrlFrom())} replace />;
};

export default ProtectedRoutes;