// https://github.com/remix-run/react-router/issues/10637#issuecomment-1802180978

import { useAppSelector } from '../redux/hooks/hooks'
import { Navigate } from 'react-router'
import { GlobalState } from '../redux/states/GlobalState'
import { Outlet } from "react-router-dom";
import Layout from './Layout';

const ProtectedRoutes = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)    

    return global.currentUser
        ?
        <Layout child={<Outlet />} />
        :
        <Navigate to={`/app/login/${encodeURIComponent(location.pathname)}`} replace />;
};

export default ProtectedRoutes;