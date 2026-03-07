import { useAdminStore } from "../../store/adminStore";

const AdminButton = () => {
  const { isAdmin, toggleAdmin } = useAdminStore();

  return (
    <div className="flex items-end justify-end my-5">
      <button
        type="button"
        onClick={toggleAdmin}
        className="text-white bg-linear-to-br from-green-400 to-blue-600 hover:bg-linear-to-bl font-medium rounded-base text-md px-4 py-2.5 text-center leading-5 rounded-xl"
      >
        {isAdmin ? "Logout Admin" : "Login as Admin"}
      </button>
    </div>
  );
};

export default AdminButton;
